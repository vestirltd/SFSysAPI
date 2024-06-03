using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;
using SFSysAPI.Services;
using SFSysAPI.Utils;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace SFSysAPI.Test.Unit.Services
{
    public class AccountsServiceTestMockHttpFluentAssertions
    {
        private readonly Mock<ILogger<AccountsService>> _mockLogger;
        private readonly Mock<IOptions<SalesforceConfig>> _mockSfConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<EncryptionUtility> _mockEncryptionUtility;
        private readonly Mock<AuthService> _mockAuthService;
        private AccountsService _accountsService;
        private HttpClient _mockClient;

        public AccountsServiceTestMockHttpFluentAssertions()
        {
            _mockLogger = new Mock<ILogger<AccountsService>>();
            _mockSfConfig = new Mock<IOptions<SalesforceConfig>>();
            _mockAuthService = new Mock<AuthService>();
            _mockEncryptionUtility = new Mock<EncryptionUtility>("mock_encryption_key");

            var sfConfig = new SalesforceConfig
            {
                UserName = "mock_username",
                Password = "#bPG+QT3Z7C9NcO58FzYe6A==",
                SecurityToken = "#R1Oss7km+hlKjmSfcEK/+zeUAPFZCgbw/D5M2a5Dx4c=",
                ConsumerKey = "mock_encrypted_consumer_key",
                ConsumerSecret = "#TjH320euqFqRRILL17WpMVEaGQ/9gqs67LDklJH84qpydQCcioL1XuZEg7Ouhp2JpJd15CI9qAJjtWHe6tpuowKmN+4i/w80",
                tokenUrl = "https://mockurl.com",
                //serviceUrl = "https://stsl-dev-ed.my.salesforce.com",
                serviceUrl = "https://mockuri/",
                basePath = "/services/data/v56.0",
                queryPath = "/query",
                enKey = "DPTheCSharpGuru"
            };

            _mockSfConfig.Setup(s => s.Value).Returns(sfConfig);
        }

        [Fact]
        public async Task AccountsServiceGetAccountsResponseSuccess()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var mockAuthService = fixture.Freeze<Mock<IAuthService>>();
            mockAuthService.Setup(auth => auth.GetAccessToken()).ReturnsAsync("mock_token");
            
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"../../../../SFSysAPI.Test.Unit/Data/Accounts/SFResponseSuccess.json";
            string filePath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

            string jsonContent = File.ReadAllText(filePath);
            
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent)
            };

            MockHttpClientAsObjectResponse(mockResponse);
            
            _accountsService = new AccountsService(_mockLogger.Object, mockAuthService.Object, _mockSfConfig.Object, _mockClient);
            List<GetAccountResponse> response = await _accountsService.GetAccounts();

            var actualResponseString = JsonConvert.SerializeObject(response);
            
            string relativePathExpected = @"../../../../SFSysAPI.Test.Unit/Data/Accounts/SFResponseSuccessExpected.json";
            string filePathExpected = Path.GetFullPath(Path.Combine(currentDirectory, relativePathExpected));
            string expectedContent = File.ReadAllText(filePathExpected);
            
            Assert.NotNull(actualResponseString);
            
            JToken actualJToken = JToken.Parse(actualResponseString);
            JToken expectedJToken = JToken.Parse(expectedContent);
            
            actualJToken.Should().BeEquivalentTo(expectedJToken);
        }
           
        private void MockHttpClientAsObjectResponse(HttpResponseMessage mockResponse)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                //Below code is to mock specific HTTP Method.
                //.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.Method==HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
                //Below code is to mock for any HTTP Method
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);
            _mockClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClient.BaseAddress = new Uri("https://mockuri/");
        }
    }
}
