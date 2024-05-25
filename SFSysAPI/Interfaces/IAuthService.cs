using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFSysAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GetAccessToken();
    }
}