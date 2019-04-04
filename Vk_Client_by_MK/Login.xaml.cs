using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.AudioBypassService.Extensions;
using Vk_Client_by_MK;
using Microsoft.Extensions.DependencyInjection;

namespace VKMessenger_by_MK
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static VkApi api = new VkApi();
        public static string username;
        public static string password;

        public Login()
        {
            var servies = new ServiceCollection();
            servies.AddAudioBypass();
            api = new VkApi(servies);

            try
            {
                string docPath = Environment.CurrentDirectory;
                string[] lines = File.ReadAllLines(Path.Combine(docPath, "LoginData.txt"));
                var login = lines[0];
                var password = lines[1];                
                Auth(login, password, ref api);                
                MainWindow main = new MainWindow();
                Close();
                main.Show();                
            }
            catch
            {                
                InitializeComponent();
            }
        }

        private void Sign_In_Clicked(object sender, RoutedEventArgs e)
        {
            username = Username.Text;
            password = Password.Password.ToString();

            try
            {
                Auth(username, password, ref api);
                MainWindow main = new MainWindow();
                this.Close();
                main.Show();
                if ((bool)Remember.IsChecked)
                {
                    Save_Data(username, password);
                }
            }
            catch (ArgumentNullException)
            {
                TwoFactWin codewin = new TwoFactWin();
                codewin.Show();
                Close();
            }
            catch (Exception k)
            {
                Status.Content = "Check your login and password";
                MessageBox.Show(k.ToString());
            }
        }
    

        private void Username_Given(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                username = Username.Text;
                password = Password.Password.ToString();

                try
                {
                    Auth(username, password, ref api);
                    MainWindow main = new MainWindow();
                    this.Close();
                    main.Show();
                    if ((bool)Remember.IsChecked)
                    {
                        Save_Data(username, password);
                    }
                }
                catch (ArgumentNullException)
                {
                    TwoFactWin codewin = new TwoFactWin();
                    codewin.Show();
                    Close();
                }
                catch (Exception k)
                {
                    Status.Content = "Check your login and password";
                    MessageBox.Show(k.ToString());
                }
            }
        }

        private void Password_Given(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                username = Username.Text;
                password = Password.Password.ToString();
                try
                {
                    Auth(username, password, ref api);
                    MainWindow main = new MainWindow();
                    this.Close();
                    main.Show();
                    if ((bool)Remember.IsChecked)
                    {
                        Save_Data(username, password);
                    }
                }
                catch (ArgumentNullException)
                {
                    TwoFactWin codewin = new TwoFactWin();
                    codewin.Show();
                    Close();
                }
                catch (Exception k)
                {
                    Status.Content = "Check your login and password";
                    MessageBox.Show(k.ToString());
                }
            }
        }        

        public void Auth(string username, string password, ref VkApi api)
        {
            api.Authorize(new ApiAuthParams
            {
                ApplicationId = 6723320,
                Login = username,
                Password = password,
                Settings = Settings.All                
            });
        }

        private static void Save_Data(string login, string password)
        {
            string docPath = Environment.CurrentDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "LoginData.txt"), true))
            {
                outputFile.WriteLine(login);
                outputFile.WriteLine(password);
            }
        }
    }
}
