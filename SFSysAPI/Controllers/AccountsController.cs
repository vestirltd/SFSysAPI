using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;

namespace SFSysAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private IAccountsService _accountsService;
        private ILogger<AccountsController> _logger;

        public AccountsController(IAccountsService accountsService, ILogger<AccountsController> logger)
        {
            this._accountsService = accountsService;
            this._logger = logger;
        }

        [HttpGet]
        //[Route("/accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            //System.Console.WriteLine("Request received for Get Accounts");
            _logger.LogInformation("Info ILogger: Request received for Get Accounts");
            List<GetAccountResponse> accounts = new List<GetAccountResponse>();
            accounts = await _accountsService.GetAccounts();
            return Ok(accounts);
        }
    }
}