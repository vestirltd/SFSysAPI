using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFSysAPI.Controllers;
using SFSysAPI.Interfaces;
using SFSysAPI.Services;
using Xunit;
using AutoFixture.Xunit2;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFSysAPI.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace SFSysAPI.Tests.UnitTests
{
    public class AccountsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IAccountsService> _mockAccountsService;
        private readonly Mock<ILogger<AccountsController>> _mockLogger;
        private readonly AccountsController _accountsController;

        public AccountsControllerTests()    
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockAccountsService = _fixture.Freeze<Mock<IAccountsService>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<AccountsController>>>();
            _accountsController = new AccountsController(_mockAccountsService.Object, _mockLogger.Object);
        }
        [Fact]
        public async Task GetAccounts_ReturnOkResult_WithListOfAccounts()
        {
            //Arrange
            var mockAccounts = _fixture.CreateMany<GetAccountResponse>(10).ToList();
            _mockAccountsService.Setup(service => service.GetAccounts())
                            .ReturnsAsync(mockAccounts);
            var result = await _accountsController.GetAccounts();
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<GetAccountResponse>>(OkResult.Value);
            Assert.Equal(10, returnValue.Count);
        }
    }
}