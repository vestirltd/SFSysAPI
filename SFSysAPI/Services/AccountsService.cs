//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;
using Salesforce.Force;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Components.Web;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Security;
//using Salesforce.Common.Models;

namespace SFSysAPI.Services
{
    public class AccountsService : IAccountsService
    {
        private ILogger<AccountsService> _logger;
        private SalesforceConfig _sfConfig;
        private EncryptionUtility _encryptionUtility;
        private HttpClient _httpClient;
        public AccountsService(ILogger<AccountsService> logger, IOptions<SalesforceConfig> sfConfig, HttpClient httpClient)
        {
            _logger = logger;
            _sfConfig = sfConfig.Value;
            _encryptionUtility = new EncryptionUtility(_sfConfig.enKey);
            _httpClient = httpClient;
        }
        async Task<List<GetAccountResponse>> IAccountsService.GetAccounts() //TODO Why IGetAccountsService.GetAccounts() is coming instead of just GetAccounts???
        {
            string token = await GetAccessToken();

            _logger.LogInformation("==============================================");
            _logger.LogInformation("Auth token received successfully from Salesforce");
            _logger.LogCritical("ILogger: Auth Token Value : " + token);

            EncryptionService es = new EncryptionService("myKey");
            string encryptedString = es.Encrypt("Hello");
            _logger.LogInformation("Encrypted Value " + encryptedString);

            List<GetAccountResponse> accts = new List<GetAccountResponse>();
            SFAccountResponse sfResponse = new SFAccountResponse();
            string query = "?q=SELECT+Name,AccountNumber,Site,Phone+from+Account";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_sfConfig.serviceUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpContent emptyContent = new StringContent("");
            HttpResponseMessage responseMessage = await httpClient.GetAsync(_sfConfig.basePath + _sfConfig.queryPath + query);
            if (responseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("Success response received from SF for account records");
                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                _logger.LogInformation("Response Content " + responseContent);
                //accts = JsonSerializer.Deserialize<SFAccountResponse>(responseContent);
                sfResponse = JsonSerializer.Deserialize<SFAccountResponse>(responseContent);
                _logger.LogInformation("Parsed Response Content " + sfResponse);

                foreach (var accountRecord in sfResponse.records)
                {
                    //GetAccountResponse acct = new GetAccountResponse();
                    //acct.Name = accountRecord.Name;
                    accts.Add(new GetAccountResponse
                    {
                        Name = accountRecord.Name,
                        Site = "http://testurl.com"
                    });
                }
                _logger.LogInformation("Number of accounts returned: " + accts.Count());
                return accts;
            }
            else
            {
                _logger.LogError("Error response received from Salesforce while retrieving account records");
                _logger.LogError(await responseMessage.Content.ReadAsStringAsync());
                return null;
            }
        }

        public async Task<string> GetAccessToken()
        { //TODO Need to move this to Client file
          //using (HttpClient client = new HttpClient())
          //{
            string Username = _sfConfig.UserName;
            string Password = _encryptionUtility.Decrypt(_sfConfig.Password);
            string SecurityToken = _encryptionUtility.Decrypt(_sfConfig.SecurityToken);
            string ConsumerKey = _encryptionUtility.Decrypt(_sfConfig.ConsumerKey);
            string ConsumerSecret = _encryptionUtility.Decrypt(_sfConfig.ConsumerSecret);

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", ConsumerKey),
                    new KeyValuePair<string, string>("client_secret", ConsumerSecret),
                    new KeyValuePair<string, string>("username", Username),
                    new KeyValuePair<string, string>("password", Password + SecurityToken)
                });
            string formString = await content.ReadAsStringAsync();

            var fullUrl = _sfConfig.tokenUrl + '?' + formString;

            HttpContent emptyContent = new StringContent("");
            HttpResponseMessage response = await _httpClient.PostAsync(fullUrl, emptyContent);
            //HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage());

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Auth response received");
                string responseContent = await response.Content.ReadAsStringAsync();
                SalesforceAuthResponseModel resp = JsonSerializer.Deserialize<SalesforceAuthResponseModel>(responseContent);
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++");
                Console.WriteLine(resp.access_token);
                return resp.access_token;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return null;
            }
        }
    }
}