//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
namespace SFSysAPI.Services
{
    public class AccountsService : IAccountsService
    {
        private ILogger<AccountsService> _logger;
        private IAuthService _authService;
        private HttpClient _httpClient;
        private SalesforceConfig _sfConfig;
        public AccountsService(ILogger<AccountsService> logger,
                        IAuthService authService,
                        IOptions<SalesforceConfig> sfConfig,
                        HttpClient httpClient)
        {
            _logger = logger;
            _authService = authService;
            _sfConfig = sfConfig.Value;
            _httpClient = httpClient;
        }
        public async Task<List<GetAccountResponse>> GetAccounts() //TODO Why IGetAccountsService.GetAccounts() is coming instead of just GetAccounts???
        {
            //string token = await GetAccessToken();
            string token = await _authService.GetAccessToken();

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
    }
}