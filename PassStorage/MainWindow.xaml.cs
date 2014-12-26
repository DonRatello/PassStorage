using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using PassStorage.Classes;

namespace PassStorage
{
    internal enum Screen
    {
        LOGIN,
        MASTER,
        PASSWORDS
    }

    public partial class MainWindow : Window
    {
        private Vault vault;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            vault = new Vault();
            setScreen(Screen.LOGIN);
        }

        private void btnEnterLogin_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("PASS HASH: " + Hash.hash("janosik"));
            if (Hash.check(txtEnterPassword.Password, Vault.ENTER))
            {
                setScreen(Screen.MASTER);
            }
            else
            {
                lbWrongPassword.Visibility = Visibility.Visible;
            }
        }

        private void btnMasterLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtMasterPassword.Password.Length < 8)
            {
                MessageBox.Show("Master password cannot be shorter than 8 chars!", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            vault.master = txtMasterPassword.Password;
            vault.ReadPasswords();
            vault.DecodePasswords();
            listPasswords.ItemsSource = vault.getPasswordTitles();
            setScreen(Screen.PASSWORDS);
        }

        private void setScreen(Screen screen)
        {
            switch (screen)
            {
                case Screen.LOGIN:
                {
                    gridLogin.Visibility = System.Windows.Visibility.Visible;
                    gridMaster.Visibility = System.Windows.Visibility.Hidden;
                    gridPasswords.Visibility = System.Windows.Visibility.Hidden;
                    break;
                }
                case Screen.MASTER:
                {
                    gridLogin.Visibility = System.Windows.Visibility.Hidden;
                    gridMaster.Visibility = System.Windows.Visibility.Visible;
                    gridPasswords.Visibility = System.Windows.Visibility.Hidden;
                    break;
                }
                case Screen.PASSWORDS:
                {
                    gridLogin.Visibility = System.Windows.Visibility.Hidden;
                    gridMaster.Visibility = System.Windows.Visibility.Hidden;
                    gridPasswords.Visibility = System.Windows.Visibility.Visible;
                    break;
                }
            }
        }

        private void listPasswords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Pass details = vault.getPassInfoById(listPasswords.SelectedIndex);
                detailLogin.Content = details.login;
                detailPassword.Content = details.password;
                detailTitle.Content = details.title;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLoginCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(detailLogin.Content.ToString());
            MessageBox.Show("Login copied!", "Clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnPassCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(detailPassword.Content.ToString());
            MessageBox.Show("Password copied!", "Clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pass newPass = new AddWindow().AddNew();

                if (newPass == null)
                {
                    return;
                }

                int id;
                try
                {
                    id = vault.passwords.OrderByDescending(pass => pass.id).First().id + 1;
                }
                catch (Exception)
                {
                    id = 0;
                }

                newPass.id = id;
                vault.passwords.Add(newPass);
                listPasswords.ItemsSource = null;
                listPasswords.ItemsSource = vault.getPasswordTitles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btnWritePasswords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vault.WritePasswords();
                MessageBox.Show("Write completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //vault.WritePasswords();
        }
    }
}
