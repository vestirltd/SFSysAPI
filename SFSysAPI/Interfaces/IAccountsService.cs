using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFSysAPI.Models;

namespace SFSysAPI.Interfaces
{
    public interface IAccountsService
    {
        public Task<List<GetAccountResponse>> GetAccounts();
    }
}