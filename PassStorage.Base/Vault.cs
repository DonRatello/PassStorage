using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PassStorage.Base
{
    public class Vault : IDisposable
    {
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public bool VaultOpen { get; set; }
        public bool PasswordsReady { get; set; }

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

            Thread thread = new Thread(ReadPasswords);
            thread.Start();
        }

        private void ReadPasswords()
        {
            string encodedPasswords;

            if (File.Exists(Config.Instance.Filename))
            {
                using (StreamReader sr = new StreamReader(Config.Instance.Filename))
                {
                    encodedPasswords = sr.ReadToEnd();
                    sr.Close();

                    //TODO
                    // ENCODE PASSWORDS
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
