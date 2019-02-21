using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VKMessenger_by_MK
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static VkApi api = new VkApi();

        public Login()
        {
            try
            {
                string docPath = Environment.CurrentDirectory;
                string[] lines = File.ReadAllLines(Path.Combine(docPath, "LoginData.txt"));
                var login = lines[0];
                var password = lines[1];                
                Auth(login, password, ref api);                
                MainWindow main = new MainWindow();
                this.Close();
                main.Show();                
            }
            catch
            {                
                InitializeComponent();
            }
        }

        private void Sign_In_Clicked(object sender, RoutedEventArgs e)
        {
            string username;
            string password;
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
            catch
            {
                Status.Content = "Check your login and password";
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

        private void Username_Given(object sender, KeyEventArgs e)
        {
            

            if (e.Key == Key.Enter)
            {
                string username;
                string password;
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
                catch
                {
                    Status.Content = "Check your login and password";
                }
            }
        }

        private void Password_Given(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string username;
                string password;
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
                catch
                {
                    Status.Content = "Check your login and password";
                }
            }
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
