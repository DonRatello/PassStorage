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
using System.Reflection;

namespace PassStorage.Classes
{
    public class Vault
    {
        public string enter;
        public string master;
        public RootList rootList;
        private string encodedPasswords;
        private string filename;
        public bool decodeCompleted = false;
        public bool saveCompleted = false;

        public Vault()
        {
            master = String.Empty;
            rootList = new RootList { data = new List<Pass>() };
            enter = Common.ReadSetting("ENTER_HASH");
            filename = Common.ReadSetting("FILENAME");
        }

        public void ReadPasswords()
        {
            decodeCompleted = false;
            rootList.data = null;
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
            RootList encodedRootList = new RootList() { data = new List<Pass>() };
            encodedRootList.date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            encodedRootList.version = Common.GetVersion();
            encodedRootList.user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            encodedRootList.computerName = Environment.MachineName;

            if (rootList.data == null)
            {
                rootList.data = new List<Pass>();
            }

            try
            {
                encodedRootList.data.AddRange(rootList.data.Select(pass => new Pass
                {
                    id = pass.id,
                    login = Crypt.EncryptRijndael(pass.login, master),
                    password = Crypt.EncryptRijndael(pass.password, master),
                    title = Crypt.EncryptRijndael(pass.title, master),
                    creationDate = pass.creationDate
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            encodedPasswords = String.Empty;
            encodedPasswords = JsonConvert.SerializeObject(encodedRootList, Formatting.Indented);
        }

        public void DecodePasswords()
        {
            try
            {
                rootList = JsonConvert.DeserializeObject<RootList>(encodedPasswords);

                foreach (var pass in rootList.data)
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
                rootList.data = new List<Pass>();
            }
        }

        public List<string> getPasswordTitles()
        {
            return rootList.data.Select(pass => pass.IsOvertime() ? $"WARNING - {pass.title}" : pass.title).ToList();
        }

        public Pass getPassInfoById(int id)
        {
            return rootList.data.FirstOrDefault(pass => pass.id == id);
        }

        public void DeletePassAt(int index)
        {
            rootList.data.RemoveAt(index);

            int id = 0;
            foreach (var pass in rootList.data)
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
            rootList.data = rootList.data.OrderBy(p => p.title).ToList();

            int id = 0;
            foreach (var pass in rootList.data)
            {
                pass.id = id;
                id++;
            }
        }

        public void BackupDecoded()
        {
            string filename = null;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "PassStorage_DecodedBackup"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "JSON file (.json)|*.json"; // Filter files by extension

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true) filename = dlg.FileName;
            else return;

            using (StreamWriter sw = new StreamWriter(filename))
            {
                string json = JsonConvert.SerializeObject(rootList, Formatting.Indented);
                sw.Write(json);
            }

            MessageBox.Show("Decoded passwords saved!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void LoadDecoded()
        {
            string filename = null;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "PassStorage_DecodedList"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "JSON file (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true) filename = dlg.FileName;
            else return;

            if (File.Exists(filename))
            {
                // Plik istnieje, odczyt
                using (StreamReader sr = new StreamReader(filename))
                {
                    encodedPasswords = sr.ReadToEnd();
                    Console.WriteLine("ENCODED PASSWORDS: " + encodedPasswords);
                    sr.Close();
                }

                rootList = JsonConvert.DeserializeObject<RootList>(encodedPasswords);
            }
        }
    }
}
