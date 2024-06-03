using Microsoft.AspNetCore.Mvc;
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
            //List<GetAccountResponse> accounts = new List<GetAccountResponse>();
            var accounts = await _accountsService.GetAccounts();
            return Ok(accounts);
        }
    }
}