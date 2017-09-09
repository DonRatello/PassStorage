using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PassStorage.Base
{
    public class Vault : IDisposable
    {
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public bool VaultOpen { get; set; }
        public bool PasswordsReady { get; set; }
        public DAL.Root Data { get; set; }

        public Vault(string pass1, string pass2)
        {
            Password1 = pass1;
            Password2 = pass2;
            PasswordsReady = false;

            CheckVaultState();
        }

        protected void CheckVaultState()
        {
            VaultOpen = Hash.Check(Password1, Config.Instance.EnterHash);

            if (!VaultOpen) return;

            Thread thread = new Thread(AnalyzePasswords);
            thread.Start();
        }

        protected void AnalyzePasswords()
        {
            string fileData;

            if (File.Exists(Config.Instance.Filename))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(Config.Instance.Filename))
                    {
                        fileData = sr.ReadToEnd();
                        sr.Close();
                    }
                    Data = JsonConvert.DeserializeObject<DAL.Root>(fileData);
                    DecodePasswords();
                }
                catch (Exception)
                {
                    Data = new DAL.Root();
                    Data.FillDefaults();
                }
            }
            else
            {
                Data = new DAL.Root();
                Data.FillDefaults();
            }

            PasswordsReady = true;
        }

        protected void DecodePasswords()
        {
            foreach (var pass in Data.data)
            {
                pass.title = Crypto.DecryptRijndael(pass.title, Password2);
                pass.login = Crypto.DecryptRijndael(pass.login, Password2);
                pass.password = Crypto.DecryptRijndael(pass.password, Password2);
            }
        }

        protected void EncodePasswords()
        {
            foreach (var pass in Data.data)
            {
                pass.title = Crypto.EncryptRijndael(pass.title, Password2);
                pass.login = Crypto.EncryptRijndael(pass.login, Password2);
                pass.password = Crypto.EncryptRijndael(pass.password, Password2);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
