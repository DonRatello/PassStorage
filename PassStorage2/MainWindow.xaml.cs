using PassStorage.Base;
using System.Timers;
using System.Windows;

namespace PassStorage2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Vault vault;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        void Initialize()
        { }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            vault = new Vault(pass1.Password, pass2.Password);

            var timer = new Timer(250);
            timer.Elapsed += (object s, ElapsedEventArgs args) =>
            {
                if (vault.PasswordsReady)
                {
                    timer.Stop();  
                    // TODO: Change screen
                }
            };
            timer.AutoReset = true;
            timer.Start();
        }
    }
}
