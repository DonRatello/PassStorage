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

        private void CheckVaultState()
        {
            VaultOpen = Hash.Check(Password1, Config.Instance.EnterHash);

            if (!VaultOpen) return;

            Thread thread = new Thread(AnalyzePasswords);
            thread.Start();
        }

        private void AnalyzePasswords()
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
                }
                catch (Exception ex)
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
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
