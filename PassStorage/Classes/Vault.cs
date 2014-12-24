using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace PassStorage.Classes
{
    class Vault
    {
        public const string ENTER = "8f4880ebd926ba499b5d3df185995de61b51543e7f19fd9ce55ae63946ac7b4c837a17b3d075aa2f156ecb814b63fa2895c6fb781088638c4f67a7e9a76ccfe8";
        public string master;
        public List<Pass> passwords;
        private string encodedPasswords;
        private const string filename = "XlfTUVdEagNmrpR15GrM.dat";

        public Vault()
        {   
            master = "  ";
            passwords = new List<Pass>(); 
            passwords = new List<Pass>
            {
                
            };
        }

        public void ReadPasswords()
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
        }

        public void WritePasswords()
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                // Koduje pusta liste i ja zapisuje
                EncodePasswords();
                sw.Write(encodedPasswords);
                sw.Flush();
                sw.Close();
            }
        }

        public void EncodePasswords()
        {
            List<Pass> encodedPasswordsList = new List<Pass>();
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
                int id = 0;
                foreach (var pass in passwords)
                {
                    pass.id = id;
                    pass.login = Crypt.DecryptRijndael(pass.login, master);
                    pass.title = Crypt.DecryptRijndael(pass.title, master);
                    pass.password = Crypt.DecryptRijndael(pass.password, master);
                    id++;
                }
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

    }
}
