using System.Text.Json;
using Microsoft.Extensions.Options;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;
using SFSysAPI.Utils;

namespace SFSysAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly SalesforceConfig _sfConfig;
        private readonly EncryptionUtility _encryptionUtility;
        private readonly ILogger<AuthService> _logger;
        private readonly HttpClient _httpClient;

        public AuthService(ILogger<AuthService> logger, IOptions<SalesforceConfig> sfConfig, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _sfConfig = sfConfig.Value;
            _encryptionUtility = new EncryptionUtility(_sfConfig.enKey);
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