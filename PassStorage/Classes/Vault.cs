using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Configuration;

namespace PassStorage.Classes
{
    public class Vault
    {
        public string enter;
        public string master;
        public List<Pass> passwords;
        private string encodedPasswords;
        private const string filename = "XlfTUVdEagNmrpR15GrM.dat";
        public bool decodeCompleted = false;
        public bool saveCompleted = false;

        public Vault()
        {
            master = String.Empty;
            passwords = new List<Pass>();
            enter = Common.ReadSetting("ENTER_HASH");
        }

        public void ReadPasswords()
        {
            decodeCompleted = false;
            passwords = null;
            Thread thread = new Thread(ReadPasswordsThread);
            thread.Start();
        }

        public void ReadPasswordsThread()
        {
            if (File.Exists(filename))
            {
                // Plik istnieje, odczyt
                using (StreamReader sr = new StreamReader(filename))
                {
                    encodedPasswords = sr.ReadToEnd();
                    Console.WriteLine("ENCODED PASSWORDS: " + encodedPasswords);
                    sr.Close();
                    DecodePasswords();
                }
            }
            else
            {
                // Plik nie istnieje, tworzenie
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    // Koduje pusta liste i ja zapisuje
                    EncodePasswords();
                    sw.Write(encodedPasswords);
                    sw.Flush();
                    sw.Close();
                }
            }

            decodeCompleted = true;
        }

        public void WritePasswords()
        {
            saveCompleted = false;
            Thread thread = new Thread(WritePasswordsThread);
            thread.Start();
        }

        public void WritePasswordsThread()
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                // Koduje pusta liste i ja zapisuje
                EncodePasswords();
                sw.Write(encodedPasswords);
                sw.Flush();
                sw.Close();
            }

            saveCompleted = true;
        }

        public void EncodePasswords()
        {
            List<Pass> encodedPasswordsList = new List<Pass>();

            if (passwords == null)
            {
                passwords = new List<Pass>();
            }

            try
            {
                encodedPasswordsList.AddRange(passwords.Select(pass => new Pass
                {
                    id = pass.id, login = Crypt.EncryptRijndael(pass.login, master), password = Crypt.EncryptRijndael(pass.password, master), title = Crypt.EncryptRijndael(pass.title, master)
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            encodedPasswords = String.Empty;
            encodedPasswords = JsonConvert.SerializeObject(encodedPasswordsList);
        }

        public void DecodePasswords()
        {
            passwords = JsonConvert.DeserializeObject<List<Pass>>(encodedPasswords);

            try
            {
                foreach (var pass in passwords)
                {
                    pass.login = Crypt.DecryptRijndael(pass.login, master);
                    pass.title = Crypt.DecryptRijndael(pass.title, master);
                    pass.password = Crypt.DecryptRijndael(pass.password, master);
                }

                Sort();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<string> getPasswordTitles()
        {
            return passwords.Select(pass => pass.title).ToList();
        }

        public Pass getPassInfoById(int id)
        {
            return passwords.FirstOrDefault(pass => pass.id == id);
        }

        public void DeletePassAt(int index)
        {
            passwords.RemoveAt(index);

            int id = 0;
            foreach (var pass in passwords)
            {
                pass.id = id;
                id++;
            }
        }

        public void Backup()
        {
            saveCompleted = false;
            Thread thread = new Thread(BackupThread);
            thread.Start();
        }

        public void BackupThread()
        {
            if (!Directory.Exists("backup"))
            {
                Directory.CreateDirectory("backup");
            }

            string backupfile = "backup_" + DateTime.Now.ToShortDateString() + ".dat";

            if (File.Exists("backup\\" + backupfile))
            {
                int count =
                    Directory.GetFiles("backup\\", backupfile.Substring(0, backupfile.LastIndexOf('.')) + "*")
                        .ToList()
                        .Count;

                backupfile = backupfile.Substring(0, backupfile.LastIndexOf('.'));
                backupfile += "_" + count + ".dat";
            }

            using (StreamWriter sw = new StreamWriter("backup\\" + backupfile))
            {
                // Koduje pusta liste i ja zapisuje
                EncodePasswords();
                sw.Write(encodedPasswords);
                sw.Flush();
                sw.Close();
            }

            saveCompleted = true;
        }

        public void Sort()
        {
            // Sorting
            passwords = passwords.OrderBy(p => p.title).ToList();

            int id = 0;
            foreach (var pass in passwords)
            {
                pass.id = id;
                id++;
            }
        }
    }
}
