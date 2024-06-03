namespace SFSysAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GetAccessToken();
    }
}