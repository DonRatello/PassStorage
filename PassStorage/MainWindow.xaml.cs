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
using System.Reflection;
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
            vault = new Vault();
            timer = new DispatcherTimer();
            menuVersion.Header = $"Version {Common.GetVersion()} Build {Common.GetLinkerTime(Assembly.GetExecutingAssembly()).ToString("yyyyMMddHHmmss")}";
            SetLoadingGridVisibility(false);
            setScreen(Screen.LOGIN);
        }

        private void btnEnterLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Hash.check(txtEnterPassword.Password, vault.enter))
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

            if (vault.getPasswordTitles().Any()) listPasswords.SelectedIndex = 0;

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
            menuBackup.IsEnabled = true;
            timer.Stop();
        }

        void timer_TickSave(object sender, EventArgs e)
        {
            if (!vault.saveCompleted) return;

            SetLoadingGridVisibility(false);
            vault.saveCompleted = false;
            menuSave.IsEnabled = true;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //vault.WritePasswords();
        }

        private void gridLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Hash.check(txtEnterPassword.Password, vault.enter))
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

        private void SetLoadingGridVisibility(bool visible)
        {
            gridLoading.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void menuAddNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pass newPass = new AddWindow().AddNew();

                if (newPass == null) return;

                int id;
                try
                {
                    id = vault.rootList.data.OrderByDescending(pass => pass.id).First().id + 1;
                }
                catch (Exception)
                {
                    id = 0;
                }

                newPass.id = id;
                vault.rootList.data.Add(newPass);
                vault.Sort();
                listPasswords.ItemsSource = null;
                listPasswords.ItemsSource = vault.getPasswordTitles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            menuSave.IsEnabled = false;
            SetLoadingGridVisibility(true);
            vault.WritePasswords();

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_TickSave;
            timer.Start();
        }

        private void menuBackup_Click(object sender, RoutedEventArgs e)
        {
            menuBackup.IsEnabled = false;
            SetLoadingGridVisibility(true);
            vault.Backup();

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_TickBackup;
            timer.Start();
        }

        private void menuBackupDecoded_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuCopyLogin_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(detailLogin.Content.ToString());
            MessageBox.Show("Login copied!", "Clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void menuCopyPassword_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(detailPassword.Content.ToString());
            MessageBox.Show("Password copied!", "Clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listPasswords.SelectedIndex == -1)
                {
                    MessageBox.Show("Please choose entry from the list", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Pass pass = vault.rootList.data.ElementAt(listPasswords.SelectedIndex);
                AddWindow add = new AddWindow();
                pass = add.Edit(pass);

                vault.Sort();
                listPasswords.ItemsSource = null;
                listPasswords.ItemsSource = vault.getPasswordTitles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        { 
            try
            {
                int index = listPasswords.SelectedIndex;

                if (MessageBox.Show($"Are you sure to delete {vault.getPassInfoById(index).title} ?", "Please confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    vault.DeletePassAt(index);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            vault.Sort();
            listPasswords.ItemsSource = null;
            listPasswords.ItemsSource = vault.getPasswordTitles();
        }

        private void menuHashGeneratorTool_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
