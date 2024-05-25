using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using SFSysAPI.Models;
using SFSysAPI.Services;

namespace SFSysAPI.Services.Tests
{
    public class AccountsServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<AccountsService>> _loggerMock;
        private readonly Mock<IOptions<SalesforceConfig>> _sfConfigMock;
        private readonly Mock<HttpClient> _httpClientMock;

        public AccountsServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _loggerMock = _fixture.Freeze<Mock<ILogger<AccountsService>>>();
            _sfConfigMock = _fixture.Freeze<Mock<IOptions<SalesforceConfig>>>();
            _httpClientMock = _fixture.Freeze<Mock<HttpClient>>();

            var sfConfig = new SalesforceConfig
            {
                enKey = "encryptionKey",
                serviceUrl = "https://example.com",
                basePath = "/services/data/v20.0",
                queryPath = "/query",
                tokenUrl = "https://login.salesforce.com/services/oauth2/token",
                UserName = "username",
                Password = "encryptedPassword",
                SecurityToken = "encryptedSecurityToken",
                ConsumerKey = "encryptedConsumerKey",
                ConsumerSecret = "encryptedConsumerSecret"
            };
            _sfConfigMock.Setup(s => s.Value).Returns(sfConfig);
        }

        [Theory, AutoData]
        public async Task GetAccounts_ReturnsAccounts_WhenResponseIsSuccessful(List<AccountRecord> accountRecords)
        {
            // Arrange
            var token = "mockToken";
            var sfResponse = new SFAccountResponse { records = accountRecords };
            var responseContent = JsonSerializer.Serialize(sfResponse);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            Func<Task<string>> getAccessTokenMock = () => Task.FromResult(token);
            _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>())).ReturnsAsync(responseMessage);

            var accountsService = new AccountsService(_loggerMock.Object, _sfConfigMock.Object, _httpClientMock.Object, getAccessTokenMock);

            // Act
            var result = await accountsService.GetAccounts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountRecords.Count, result.Count);
            for (int i = 0; i < accountRecords.Count; i++)
            {
                Assert.Equal(accountRecords[i].Name, result[i].Name);
            }
        }

        /*[Theory, AutoData]
        public async Task GetAccounts_ReturnsNull_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var token = "mockToken";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            };

            Func<Task<string>> getAccessTokenMock = () => Task.FromResult(token);
            _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>())).ReturnsAsync(responseMessage);

            var accountsService = new AccountsService(_loggerMock.Object, _sfConfigMock.Object, _httpClientMock.Object, getAccessTokenMock);

            // Act
            var result = await accountsService.GetAccounts();

            // Assert
            Assert.Null(result);
        }*/
    }
}