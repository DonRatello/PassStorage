using PassStorage.Base;
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
        }
    }
}
