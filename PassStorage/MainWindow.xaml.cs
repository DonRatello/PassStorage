using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
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
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("NEW GUID: " + Guid.NewGuid());
            vault = new Vault();
            timer = new DispatcherTimer();
            SetLoadingGridVisibility(false);
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
            MasterBtnAction();
        }

        private void MasterBtnAction()
        {
            if (txtMasterPassword.Password.Length < 8)
            {
                MessageBox.Show("Master password cannot be shorter than 8 chars!", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            vault.master = txtMasterPassword.Password;

            SetLoadingGridVisibility(true);
            vault.ReadPasswords();

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!vault.decodeCompleted) return;

            listPasswords.ItemsSource = vault.getPasswordTitles();
            SetLoadingGridVisibility(false);
            setScreen(Screen.PASSWORDS);
            vault.decodeCompleted = false;
            timer.Stop();
        }

        void timer_TickBackup(object sender, EventArgs e)
        {
            if (!vault.saveCompleted) return;

            SetLoadingGridVisibility(false);
            vault.saveCompleted = false;
            btnBackup.IsEnabled = true;
            timer.Stop();
        }

        void timer_TickSave(object sender, EventArgs e)
        {
            if (!vault.saveCompleted) return;

            SetLoadingGridVisibility(false);
            vault.saveCompleted = false;
            btnWritePasswords.IsEnabled = true;
            timer.Stop();
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
                    FocusManager.SetFocusedElement(gridMaster, txtMasterPassword);
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
                //MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            btnWritePasswords.IsEnabled = false;
            SetLoadingGridVisibility(true);
            vault.WritePasswords();

            timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += timer_TickSave;
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //vault.WritePasswords();
        }

        private void gridLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Hash.check(txtEnterPassword.Password, Vault.ENTER))
                {
                    setScreen(Screen.MASTER);
                }
                else
                {
                    lbWrongPassword.Visibility = Visibility.Visible;
                }
            }
        }

        private void gridMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MasterBtnAction();
            }
        }

        private void btnDeleteSelectedPassword_Click(object sender, RoutedEventArgs e)
        {
            int index;

            try
            {
                index = listPasswords.SelectedIndex;
                vault.DeletePassAt(index);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            listPasswords.ItemsSource = null;
            listPasswords.ItemsSource = vault.getPasswordTitles();
        }

        private void SetLoadingGridVisibility(bool visible)
        {
            gridLoading.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            btnBackup.IsEnabled = false;
            SetLoadingGridVisibility(true);
            vault.Backup();

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_TickBackup;
            timer.Start();
        }
    }
}
