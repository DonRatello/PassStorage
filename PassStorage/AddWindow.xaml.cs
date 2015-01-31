using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PassStorage.Classes;

namespace PassStorage
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private Pass newPassword;

        public AddWindow()
        {
            InitializeComponent();
            newPassword = null;
        }

        public Pass AddNew()
        {
            ShowDialog();
            return newPassword;
        }

        public Pass Edit(Pass password)
        {
            newPassword = password;

            txtTitle.Text = newPassword.title;
            txtPassword.Text = newPassword.password;
            txtLogin.Text = newPassword.login;

            ShowDialog();
            return newPassword;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (newPassword == null)
            {
                // If null means it's add operation
                newPassword = new Pass()
                {
                    title = txtTitle.Text,
                    login = txtLogin.Text,
                    password = txtPassword.Text,
                };
            }
            else
            {
                newPassword.title = txtTitle.Text;
                newPassword.password = txtPassword.Text;
                newPassword.login = txtLogin.Text;
            }

            this.Close();
        }

        private void sliderChars_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                charsCount.Content = (Math.Round(sliderChars.Value, 0)).ToString();
            }
            catch
            {

            }
        }

        private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            txtPassword.Text = Crypt.GenerateString(Int32.Parse(charsCount.Content.ToString()));
        }
    }
}
