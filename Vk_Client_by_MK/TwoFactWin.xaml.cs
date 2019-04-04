using System.IO;
using System.Threading;
using System.Windows;
using System.Drawing;
using VKMessenger_by_MK;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using System.Windows.Media.Imaging;

namespace Vk_Client_by_MK
{
    /// <summary>
    /// Логика взаимодействия для TwoFactWin.xaml
    /// </summary>
    public partial class TwoFactWin : Window
    {
        public static string code = "";

        public TwoFactWin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login.api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 6723320,
                    Login = Login.username,
                    Password = Login.password,
                    Settings = Settings.All,
                    TwoFactorAuthorization = () =>
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate { code = Code.Text; }));
                        return code;
                    }
                });
                MainWindow main = new MainWindow();
                main.Show();
                Close();    
            }
            /*catch (Exception t)
            {
                MessageBox.Show(t.ToString());
                MessageBox.Show(Code.Text);
                MessageBox.Show(Login.password);
                MessageBox.Show(Login.username);
            }*/
            catch (CaptchaNeededException ex)
            {
                Captcha captcha = new Captcha(ex);
                captcha.Show();
            }
        }
    }
}
