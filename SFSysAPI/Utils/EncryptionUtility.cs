using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Security;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using SFSysAPI.Interfaces;

namespace SFSysAPI.Services
{
    public class EncryptionUtility
    {
        private readonly byte[] _encryptionKeyBytes;
        private  readonly Encoding _encoding;

        public EncryptionUtility(string encryptionKey)
        {
            _encryptionKeyBytes = Encoding.UTF8.GetBytes(encryptionKey);
            _encoding = Encoding.UTF8;
        }

        public string Encrypt(string value)
        {
            byte[] inputBytes = _encoding.GetBytes(value);
            byte[] encryptedBytes;

            //TODO What is this using statement doing?
            //using (var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()), new Pkcs7Padding()))
            //{
                PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()), new Pkcs7Padding());
                cipher.Init(true, new KeyParameter(_encryptionKeyBytes));

                encryptedBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
                int length = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, length);
            //}

            return Convert.ToBase64String(encryptedBytes);
        }
        public string Decrypt(string encryptedValue)
        {
            byte[] encryptedBytes;
            byte[] decryptedBytes;
            if(encryptedValue.StartsWith('#')){
                encryptedBytes = Convert.FromBase64String(encryptedValue.Substring(1));

                //using (var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()), new Pkcs7Padding()))
                //{
                    PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new BlowfishEngine()), new Pkcs7Padding());
                    cipher.Init(false, new KeyParameter(_encryptionKeyBytes));

                    decryptedBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                    int length = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, decryptedBytes, 0);
                    cipher.DoFinal(decryptedBytes, length);
                //}
                return _encoding.GetString(decryptedBytes).TrimEnd('\0');
            }
            else
            {
                    return encryptedValue;
            }
        }
    }
}