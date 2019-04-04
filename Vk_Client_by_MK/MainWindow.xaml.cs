using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Security.Cryptography;
using VkNet;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace VKMessenger_by_MK
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static VkApi api = Login.api;
        public static string myname = api.Account.GetProfileInfo().FirstName;

        static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096);
        public static string pubkey = ToXmlString(RSA, false);
        public static string privkey = ToXmlString(RSA, true);

        public static string SimKeyforMes = Generate_Sim_Key();
        public static string SimKeyforCom = Generate_Sim_Key();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.5);
            ta.DecelerationRatio = 0.5;
            ta.To = new Thickness(0, 0, 0, 0);
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = new Thickness(500, 0, 0, 0);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = new Thickness(0, 0, 500, 0);
            }
                 (e.Content as Page).BeginAnimation(MarginProperty, ta);
        }

        private static string ToXmlString(RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
        }

        private static string Generate_Sim_Key()
        {
            var keygen = new CipherKeyGenerator();
            keygen.Init(new KeyGenerationParameters(new SecureRandom(), 256));
            var key = Convert.ToBase64String(keygen.GenerateKey());
            return key;
        }

        private void Message_Button_Click(object sender, RoutedEventArgs e)
        {
            Pages.NavigationService.Navigate(new Chat_with_List());
            Welcome.Content = "";
        }
    }
}
