using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using SFSysAPI.Models;
using Microsoft.VisualBasic;
using AutoFixture;
using AutoFixture.AutoMoq;
using SFSysAPI.Services;
using SFSysAPI.Interfaces;

namespace SFSysAPI.Services.Tests
{
    public class AccountsServiceTest_GetAcounts_HTTPMock
    {
        private readonly Mock<ILogger<AccountsService>> _mockLogger;
        private readonly Mock<IOptions<SalesforceConfig>> _mockSfConfig;
        //private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<EncryptionUtility> _mockEncryptionUtility;
        private IAccountsService _accountsService;
        private Mock<HttpClient> _mockHttpClient;
        private HttpClient _mockClient;
        private Mock<AccountsService> _mockAccountsService;
        private readonly Fixture _fixture;

        public AccountsServiceTest_GetAcounts_HTTPMock()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _mockLogger = new Mock<ILogger<AccountsService>>();
            _mockSfConfig = new Mock<IOptions<SalesforceConfig>>();
            //_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new Mock<HttpClient>();
            _mockEncryptionUtility = new Mock<EncryptionUtility>("mock_encryption_key");


            var sfConfig = new SalesforceConfig
            {
                UserName = "mock_username",
                Password = "#bPG+QT3Z7C9NcO58FzYe6A==",
                SecurityToken = "#R1Oss7km+hlKjmSfcEK/+zeUAPFZCgbw/D5M2a5Dx4c=",
                ConsumerKey = "mock_encrypted_consumer_key",
                ConsumerSecret = "#TjH320euqFqRRILL17WpMVEaGQ/9gqs67LDklJH84qpydQCcioL1XuZEg7Ouhp2JpJd15CI9qAJjtWHe6tpuowKmN+4i/w80",
                tokenUrl = "https://mockurl.com",
                enKey = "DPTheCSharpGuru"
            };

            _mockSfConfig.Setup(s => s.Value).Returns(sfConfig);
            //var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            //_accountsService = new AccountsService(_mockLogger.Object, _mockSfConfig.Object, _mockClient);
        }

        [Fact]
        public async Task GetAccounts_ShouldReturnSuccessResponse()
        {
            // Arrange
            var sfResponse =  _fixture.Create<SFAccountResponse>();
            
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(sfResponse))
            };

            MockHttpClientAsObjectResponse(mockResponse);
            _mockAccountsService = new Mock<AccountsService>(_mockLogger.Object, _mockSfConfig.Object,_mockHttpClient);
            _mockAccountsService.Setup(x => x.GetAccessToken()).ReturnsAsync("fake-access-token");

            
            _accountsService = new AccountsService(_mockLogger.Object, _mockSfConfig.Object, _mockHttpClient.Object);
            //_accountsService.Setup
            
            // Act
            var result = await _accountsService.GetAccounts();

            // Assert
            Assert.NotNull(result);
            //Assert.Equal("mock_access_token_value", result);
        }

        /*[Fact]
        public async Task GetAccessToken_ShouldReturnNull_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            MockHttpClientAsObjectResponse(mockResponse);
            _accountsService = new AccountsService(_mockLogger.Object, _mockSfConfig.Object, _mockClient);
            // Act
            var result = await _accountsService.GetAccessToken();

            // Assert
            Assert.Null(result);
        }*/

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
