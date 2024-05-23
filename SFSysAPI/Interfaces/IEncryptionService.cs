using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFSysAPI.Interfaces
{
    public interface IEncryptionService
    {
        public string Encrypt(string value);
        public string Decrypt(string encryptedValue);
    }
}