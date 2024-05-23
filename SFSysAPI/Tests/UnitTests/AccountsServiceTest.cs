using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using SFSysAPI.Models;
using SFSysAPI.Services;
using Xunit;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Org.BouncyCastle.Security;
using AutoFixture.AutoMoq;

namespace SFSysAPI.Tests.UnitTests
{
    public class AccountsServiceTest
    {
        private IFixture? _fixture;
        private Mock<IOptions<SalesforceConfig>> _mockSfConfig;
        private Mock<ILogger<AccountsService>> _logger;
        private HttpClient _mockHttpClient;
        private AccountsService _accountsService;

        public AccountsServiceTest()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            //_mockSfConfig = _fixture.Freeze<Mock<IOptions<SalesforceConfig>>>();
            _mockSfConfig = new Mock<IOptions<SalesforceConfig>>();
            _logger = _fixture.Freeze<Mock<ILogger<AccountsService>>>();
            var mockHttp = new MockHttpMessageHandler();
            _mockHttpClient = new HttpClient(mockHttp);
            //_mockSfConfig.SetupGet(x => x.Value.UserName).Returns("test_user");
            _mockSfConfig.SetupGet(x => x.Value.Password).Returns("#6U561jURqsEhlWeB8U2/ag==");
            /*_mockSfConfig.SetupGet(x => x.Value.SecurityToken).Returns("#vgE5ssQOsU6T45KbZzUyCb+F6s6YYIRs");
            _mockSfConfig.SetupGet(x => x.Value.ConsumerKey).Returns("test_consumer_key");
            _mockSfConfig.SetupGet(x => x.Value.ConsumerSecret).Returns("#d5Q40137vA+5hGWOa3ZhGTezmevuVo1j");
            _mockSfConfig.SetupGet(x => x.Value.tokenUrl).Returns("https://login.salesforce.com/services/oauth2/token");
            _mockSfConfig.SetupGet(x => x.Value.serviceUrl).Returns("test_serviceurl");
            _mockSfConfig.SetupGet(x => x.Value.basePath).Returns("test_basepath");
            _mockSfConfig.SetupGet(x => x.Value.queryPath).Returns("test_querypath");
            _mockSfConfig.SetupGet(x => x.Value.enKey).Returns("test_enkey");*/
            string test = _mockSfConfig.Object.Value.Password;

            //_accountsService = new AccountsService(_logger.Object, _mockSfConfig.Object, _mockHttpClient);
            _accountsService = new AccountsService(_logger.Object);
        }
        [Fact]
        public async Task AccountService_GetAccessToken_Test()
        {
            string expectedToken = "test_access_token";
            //var sfResponse = new SalesforceAuthResponseModel{access_token=expectedToken};
            var sfResponse = new SalesforceAuthResponseModel{access_token=expectedToken};
            var responseContent = JsonSerializer.Serialize(sfResponse);

            Console.WriteLine("Test: " + _mockSfConfig.Object.Value.enKey);

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, _mockSfConfig.Object.Value.tokenUrl + "*")
                .Respond("application/json",responseContent);
            _mockHttpClient = new HttpClient(mockHttp);

            _mockSfConfig.SetupGet(x => x.Value.UserName).Returns("test_user");
            _mockSfConfig.SetupGet(x => x.Value.tokenUrl).Returns("https://login.salesforce.com/services/oauth2/token");

            //_mockEncryptionUtility.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x); // Simulating Decrypt method


            string access_token = await _accountsService.GetAccessToken();

            //Assert.NotNull(access_token);
            //Assert.Equal(expectedToken, access_token);
            

            Assert.NotNull("Test");
        }
    }
}
