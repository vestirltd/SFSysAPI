using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;
using System.Text.Json;

namespace SFSysAPI.Services
{
    public class ContactsService(
        ILogger<ContactsService> logger,
        IOptions<SalesforceConfig> sfConfig,
        IAuthService authService,
        HttpClient httpClient) : IContactsService
    {
        private readonly SalesforceConfig _sfConfig = sfConfig.Value;

        public async Task<List<SendContact>> GetContacts(){
            logger.LogInformation("Received request to get Contacts");

            var token = await authService.GetAccessToken();
            logger.LogInformation("Access token retrieved from Salesforce");
            string contactQuery = "SELECT+name,accountid,mobilephone,email+from+Contact";
            string fullUrl = _sfConfig.serviceUrl + _sfConfig.basePath + _sfConfig.queryPath + "/?q=" + contactQuery;
            //httpClient.BaseAddress = new Uri(fullUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,fullUrl);
            //request.Headers.Add("Authorization","Bearer "+token);
            
            var httpResponse = await httpClient.GetAsync(fullUrl);

            if (httpResponse.IsSuccessStatusCode)
            {
                string response = await httpResponse.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Contacts>(response);
                if (jsonResponse != null)
                {
                    logger.LogInformation("Number of contacts returned: " + jsonResponse.totalSize);
                    List<SendContact> sendContacts = new List<SendContact>();
                    foreach (var record in jsonResponse.records)
                    {
                        sendContacts.Add(new SendContact
                        {
                            Name = record.Name,
                            Email = record.Email
                        });
                    }
                    return sendContacts;
                }
                return null;
            }
            else
            {
                logger.LogInformation(("Error Response received from Salesforce"));
                return null;
            }
        }
    }
}