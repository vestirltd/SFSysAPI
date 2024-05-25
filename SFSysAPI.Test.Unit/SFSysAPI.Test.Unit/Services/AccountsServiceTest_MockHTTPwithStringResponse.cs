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

namespace SFSysAPI.Services.Tests
{
    public class AccountsServiceTest_MockHTTPwithStringResponse
    {
        private readonly Mock<ILogger<AccountsService>> _mockLogger;
        private readonly Mock<IOptions<SalesforceConfig>> _mockSfConfig;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<EncryptionUtility> _mockEncryptionUtility;
        private AccountsService _accountsService;
        private HttpClient _mockClient;

        public AccountsServiceTest_MockHTTPwithStringResponse()
        {
            _mockLogger = new Mock<ILogger<AccountsService>>();
            _mockSfConfig = new Mock<IOptions<SalesforceConfig>>();
            //_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            //_mockClient = new HttpClient>();
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
        public async Task GetAccessToken_ShouldReturnAccessToken_WhenResponseIsSuccessful()
        {
            // Arrange
            //var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StringContent(JsonSerializer.Serialize(new { access_token = "mock_access_token" }))
            //};

            string mockResponse = "{\"access_token\":\"mock_access_token_value\",\"instance_url\":\"https://stsl-dev-ed.my.salesforce.com\",\"id\":\"https://login.salesforce.com/id/00D4L000000hekIUAQ/0054L000001zwkCQAQ\",\"token_type\":\"Bearer\",\"issued_at\":\"1716369710328\",\"signature\":\"wO+p7MMoTaNFQGapKto6IT6JE39WZtcGq9VX9KMHTi0=\"}";
            MockHttpClientWithStringResponse(mockResponse);
            _accountsService = new AccountsService(_mockLogger.Object, _mockSfConfig.Object, _mockClient);
             /*_mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "PostAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);*/
            /*_mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "PostAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsNull<CancellationToken>() // Use ItExpr.IsNull<CancellationToken> instead of null
            )
            .ReturnsAsync(mockResponse);*/

            // Act
            var result = await _accountsService.GetAccessToken();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mock_access_token_value", result);
        }

        private void MockHttpClientWithStringResponse(string responseJson)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
               //Below code is to mock specific HTTP Method.
               //.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.Method==HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
               //Below code is to mock for any HTTP Method
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseJson),
               });
            _mockClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockClient.BaseAddress = new Uri("https://mockuri/");
        }

        /*[Fact]
        public async Task GetAccessToken_ShouldReturnNull_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };

            _mockHttpClient
                .Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            _mockEncryptionUtility.Setup(x => x.Decrypt("mock_encrypted_password")).Returns("mock_password");
            _mockEncryptionUtility.Setup(x => x.Decrypt("mock_encrypted_security_token")).Returns("mock_security_token");
            _mockEncryptionUtility.Setup(x => x.Decrypt("mock_encrypted_consumer_key")).Returns("mock_consumer_key");
            _mockEncryptionUtility.Setup(x => x.Decrypt("mock_encrypted_consumer_secret")).Returns("mock_consumer_secret");

            // Act
            var result = await _accountsService.GetAccessToken();

            // Assert
            Assert.Null(result);
        }*/
    }
}
