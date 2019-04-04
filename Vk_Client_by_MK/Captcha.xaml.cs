using System.Windows;
using System.IO;
using System.Threading;
using System.Drawing;
using VKMessenger_by_MK;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using System.Windows.Media.Imaging;

namespace Vk_Client_by_MK
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : Window
    {
        public Captcha(CaptchaNeededException ex)
        {
            System.Uri url = ex.Img;
            System.Net.WebClient client = new System.Net.WebClient();
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(client.DownloadData(url)));
            BitmapImage bitmap = ToWpfImage(img);
            CaptchaImage.Height = bitmap.Height;
            CaptchaImage.Width = bitmap.Width;
            CaptchaImage.Source = ToWpfImage(img);
            long captcha_sid = ex.Sid;
            InitializeComponent();
        }

        public static BitmapImage ToWpfImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();
            return ix;
        }

        private void SendCode_Clicked(object sender, RoutedEventArgs e)
        {
            string captcha_key = CapCode.Text;
            Login.api.Authorize(new ApiAuthParams
            {
                ApplicationId = 6723320,
                Login = Login.username,
                Password = Login.password,
                Settings = Settings.All,
                CaptchaSid = captcha_sid,
                CaptchaKey = CapCode.Text,
                TwoFactorAuthorization = () =>
                {   
                    return TwoFactWin.code;
                }

            });
        }
    }
}
