using SFSysAPI.Models;

namespace SFSysAPI.Interfaces
{
    public interface IAccountsService
    {
        public Task<List<GetAccountResponse>> GetAccounts();
    }
}