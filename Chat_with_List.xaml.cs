using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Linq;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using System.Windows.Media;

namespace VKMessenger_by_MK
{
    /// <summary>
    /// Логика взаимодействия для Chat_with_List.xaml
    /// </summary>
    public partial class Chat_with_List : Page
    {
        int idsob;

        public static string pubkey = MainWindow.pubkey;

        public Chat_with_List()
        {
            InitializeComponent();            
            CreateFriendsList();
            Console.WriteLine(MainWindow.privkey);
        }



        public void CreateFriendsList()
        {
            FullFriendList.Items.Add(MainWindow.api.Account.GetProfileInfo().FirstName + " " + MainWindow.api.Account.GetProfileInfo().LastName + " ID:" + MainWindow.api.UserId);

            var friend_list = MainWindow.api.Friends.Get(new FriendsGetParams
            {
                Order = FriendsOrder.Hints,
                Fields = ProfileFields.FirstName,
                Count = 6000,
                NameCase = NameCase.Nom
            });
            foreach (var friend in friend_list)
            {
                FullFriendList.Items.Add(friend.FirstName + " " + friend.LastName + " ID:" + friend.Id);
            }
        }

        private void IDSearch_Is_Focused(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IDSearch.Text = "";
            this.IDSearch.Foreground = Brushes.Black;
        }

        private void IDSearch_Lost_Focus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IDSearch.Text = "Find person with ID";
            this.IDSearch.Foreground = Brushes.Gray;
        }

        private void ID_Entered(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                idsob = Convert.ToInt32(IDSearch.Text);
                var StartDialogThread = new Thread(() => StartDialog(idsob, MainWindow.api, ref pubkey, MainWindow.privkey, MainWindow.SimKeyforMes));
                StartDialogThread.Start();
                
            }
        }

        private void ElementSelected(object sender, MouseButtonEventArgs e)
        {
            
            string userdata = FullFriendList.SelectedValue.ToString();
            idsob = Convert.ToInt32(userdata.Substring(userdata.IndexOf("ID:") + 3));            
            var StartDialogThread = new Thread(() => StartDialog(idsob, MainWindow.api, ref pubkey, MainWindow.privkey, MainWindow.SimKeyforMes));
            StartDialogThread.Start();
        }       

        private void StartDialog(int idsob, VkApi api, ref string pubkey, string privkey, string SimKey)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "";
            }));

            var GetMesThread = new Thread(Get_Mes);
            bool me_or_him = true;

            Random random = new Random();

            if (!Check_Key_Without_GUI(MainWindow.api, idsob))
            {
                api.Messages.Send(new MessagesSendParams
                {
                    UserId = idsob,
                    RandomId = random.Next(99999),
                    Message = "Using VKMessenger by MK"
                });
            }

            var newpubkey = ChangeKeys(api, pubkey, idsob, ref me_or_him);
            pubkey = newpubkey;

            if (me_or_him)
            {                
                Send_Sim_Key(api, idsob, SimKey, pubkey);
            }
            else
            {
                SimKey = Get_Sim_Key(api, idsob, privkey);
                MainWindow.SimKeyforMes = SimKey;
            }

            string npub = pubkey;            

            object mesargums = new object[] { api, idsob, SimKey , pubkey, privkey};

            GetMesThread.SetApartmentState(ApartmentState.STA);
            GetMesThread.IsBackground = true;
            GetMesThread.Start(mesargums);
        }

        private void Send_Sim_Key(VkApi api, int idsob, string SimKey, string pubkey)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Идет отправка симметричного ключа..." + Environment.NewLine;
            }));            
            var random = new Random();
            var randid = random.Next(99999);
            var CryptedSimKey = RSAEncryption(SimKey, pubkey);
            api.Messages.Send(new MessagesSendParams
            {
                UserId = idsob,
                RandomId = randid,
                Message = CryptedSimKey
            });
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Ключ успешно отправлен!!!" + Environment.NewLine;
            }));            
        }

        private string Get_Sim_Key(VkApi api, int idsob, string privkey)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Получаем ключ симметричного шифрования..." + Environment.NewLine;
            }));
            string newkey;
            while (true)
            {
#pragma warning disable CS0618 // Тип или член устарел
                MessagesGetObject getDialogs = api.Messages.GetDialogs(new MessagesDialogsGetParams
                {
                    Count = 200
                });
#pragma warning restore CS0618 // Тип или член устарел
                Thread.Sleep(500);
                var curmessage = "";
                var state = false;
                int i;
                for (i = 0; i < 200; i++)
                {
                    if (getDialogs.Messages[i].UserId == idsob)
                    {
                        state = (bool)getDialogs.Messages[i].Out;
                        curmessage = getDialogs.Messages[i].Body;
                        break;
                    }
                }

                if (state == false && curmessage.Substring(0, 13) != "<RSAKeyValue>")
                {
                    newkey = RSADecryption(curmessage, privkey);
                    break;
                }
            }

            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Ключ получен!!!" + Environment.NewLine;
            }));            
            return newkey;
        }

        public static string RSAEncryption(string strText, string pubkey)
        {
            var publicKey = pubkey;

            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    FromXmlString(rsa, publicKey);

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string RSADecryption(string strText, string privkey)
        {
            var privateKey = privkey;

            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    var base64Encrypted = strText;

                    // server decrypting data with private key                    
                    FromXmlString(rsa, privateKey);

                    var npriv = privateKey;
                    
                    Console.WriteLine(ToXmlString(rsa, true));
                    
                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        private static void FromXmlString(RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "Exponent":
                            parameters.Exponent = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "P":
                            parameters.P = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "Q":
                            parameters.Q = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "DP":
                            parameters.DP = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "DQ":
                            parameters.DQ = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "InverseQ":
                            parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "D":
                            parameters.D = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
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

        private void Send_Message(VkApi api, int idsob, string message)
        {
            Array mesargar = new object[3];

            var random = new Random();
            var randid = random.Next(999999);

            string crmessage = Encryption(message, MainWindow.SimKeyforMes);
            try
            {
                api.Messages.Send(new MessagesSendParams
                {
                    UserId = idsob,
                    RandomId = randid,
                    Message = crmessage
                });
            }
            catch
            {
                try
                {
                    api.Messages.Send(new MessagesSendParams
                    {
                        UserId = idsob,
                        RandomId = randid,
                        Message = crmessage
                    });
                }
                catch
                {
                    api.Messages.Send(new MessagesSendParams
                    {
                        UserId = idsob,
                        RandomId = randid,
                        Message = crmessage
                    });
                }
            }
        }

        private void SendMes(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send_Message(MainWindow.api, idsob, MyMes.Text);
                Chat.Text += MainWindow.myname + ":" + MyMes.Text + Environment.NewLine;
                MyMes.Text = "";
                Chat.Focus();
                Chat.CaretIndex = Chat.Text.Length;
                Chat.ScrollToEnd();
                MyMes.Focus();
            }
            if (e.Key == Key.Enter && e.Key == Key.LeftShift)
            {
                MyMes.Text += Environment.NewLine;
            }
        }

        [STAThread]
        private void Get_Mes(object mesargums)
        {
            string sobname = MainWindow.api.Users.Get(new long[] { idsob }).FirstOrDefault().FirstName;
            var predmessage = "zh";
            Array mesargar = new object[3];
            mesargar = (Array)mesargums;
            var get = (VkApi)mesargar.GetValue(0);
            var userid = (int)mesargar.GetValue(1);
            var SimKey = (string)mesargar.GetValue(2);
            var pubkey = (string)mesargar.GetValue(3);
            var privkey = (string)mesargar.GetValue(4);

            bool messtate = false;
            while (true)
            {
                var curmessage = "";
                MessagesGetObject getDialogs;
                try
                {
#pragma warning disable CS0618 // Тип или член устарел
                    getDialogs = get.Messages.GetDialogs(new MessagesDialogsGetParams
                    {
                        Count = 200
                    });
#pragma warning restore CS0618 // Тип или член устарел
                }
                catch
                {
#pragma warning disable CS0618 // Тип или член устарел
                    getDialogs = get.Messages.GetDialogs(new MessagesDialogsGetParams
                    {
                        Count = 200
                    });
#pragma warning restore CS0618 // Тип или член устарел
                }
                for (var i = 0; i < 200; i++)
                {
                    if (getDialogs.Messages[i].UserId == userid)
                    {
                        curmessage = getDialogs.Messages[i].Body;
                        messtate = (bool)getDialogs.Messages[i].Out;
                        break;
                    }
                }                

                /*if (curmessage == "Using VKMessenger by MK")
                {
                    pubkey = Get_Key(MainWindow.api, idsob);
                    Send_Key(MainWindow.api, pubkey, idsob, false);
                    SimKey = Get_Sim_Key(MainWindow.api, idsob, privkey: privkey);                    
                }*/

                string decmessage = curmessage;

                try
                {
                    decmessage = Decryption(curmessage, SimKey);
                }
                catch
                {
                    decmessage = curmessage;
                }

                if (predmessage != decmessage && !messtate)
                {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate {
                        Chat.Text += sobname + ":" + predmessage + Environment.NewLine; Chat.Focus();
                        Chat.CaretIndex = Chat.Text.Length;
                        Chat.ScrollToEnd();
                        MyMes.Focus();
                    }));
                }
                predmessage = decmessage;
                Thread.Sleep(50);
            }
        }

        private static string Decryption(string curmessage, string key)
        {
            var decryptor = new CamelliaEngine();

            var strkey = key;
            ICipherParameters param = new KeyParameter(Convert.FromBase64String(strkey));
            decryptor.Init(false, param);

            byte[] nbts = Convert.FromBase64String(curmessage);
            var ndbts = new byte[nbts.Length];
            if (ndbts.Length <= 16)
            {
                decryptor.ProcessBlock(nbts, 0, ndbts, 0);
                return Encoding.UTF8.GetString(ndbts);
            }

            for (int i = 0; i < ndbts.Length; i += 16)
            {
                decryptor.ProcessBlock(nbts, i, ndbts, i);
            }

            return Encoding.UTF8.GetString(ndbts);
        }

        private static string Encryption(string data, string key)
        {
            var encryptor = new CamelliaEngine();
            var strkey = key;
            ICipherParameters param = new KeyParameter(Convert.FromBase64String(strkey));
            encryptor.Init(true, param);
            var strlengthbytes = Encoding.UTF8.GetByteCount(data);

            if (strlengthbytes > 16 && strlengthbytes % 16 != 0)
            {
                for (int i = 0; i < 16 - strlengthbytes % 16; i++)
                {
                    data += " ";
                }

            }

            var encdata = Encoding.UTF8.GetBytes(data);

            var decmes = "";
            if (encdata.Length < 16)
            {
                for (int i = 0; i < 16 - encdata.Length; i++)
                {
                    data += " ";
                }

                encdata = Encoding.UTF8.GetBytes(data);
                byte[] decdata = new byte[encdata.Length];
                encryptor.ProcessBlock(encdata, 0, decdata, 0);
                decmes = Convert.ToBase64String(decdata);
            }


            if (encdata.Length > 16)
            {

                byte[] decdata = new byte[encdata.Length];
                for (int i = 0; i < encdata.Length; i += 16)
                {
                    encryptor.ProcessBlock(encdata, i, decdata, i);
                    if (i + 16 > encdata.Length)
                    {
                        break;
                    }
                }

                decmes = Convert.ToBase64String(decdata);
            }

            return decmes;
        }    

        private string ChangeKeys(VkApi api, string pubkey, int idsob, ref bool me_or_him)
        {
            string newpubkey;
            if (Check_Key(api, idsob) == false)
            {
                Send_Key(api, pubkey, idsob, true);
                newpubkey = Get_Key(api, idsob);
                me_or_him = true;
            }
            else
            {
                me_or_him = false;
                newpubkey = Get_Key(api, idsob);
                Send_Key(api, pubkey, idsob, false);
            }

            return newpubkey;
        }

        private bool Check_Key(VkApi api, int idsob)
        {
            bool pr = true;

            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Идет проверка на присутствие ключа в чате..." + Environment.NewLine;
            }));

#pragma warning disable CS0618 // Тип или член устарел
            var getDialogs = api.Messages.GetDialogs(new MessagesDialogsGetParams
            {
                Count = 200
            });
#pragma warning restore CS0618 // Тип или член устарел
            var curmessage = "";
            var state = false;
            for (var i = 0; i < 200; i++)
            {
                if (getDialogs.Messages[i].UserId == idsob)
                {
                    state = (bool)getDialogs.Messages[i].Out;
                    curmessage = getDialogs.Messages[i].Body;
                    break;
                }
            }

            if (curmessage.Length < 13)
            {
                pr = false;
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Собеседник не отправил Вам свой ключ((" + Environment.NewLine;
                }));

                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Опять вся надежда на Вас!!!" + Environment.NewLine;
                }));                

                return false;
            }

            if (curmessage.Substring(0, 13) == "<RSAKeyValue>" && state == false)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Ключ есть!" + Environment.NewLine;
                }));                

                return true;
            }

            if (pr)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Собеседник не отправил Вам свой ключ((" + Environment.NewLine;
                }));

                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Опять вся надежда на Вас!!!" + Environment.NewLine;
                }));                

                return false;
            }

            return false;
        }

        private bool Check_Key_Without_GUI(VkApi api, int idsob)
        {
            bool pr = true;

#pragma warning disable CS0618 // Тип или член устарел
            MessagesGetObject getDialogs = api.Messages.GetDialogs(new MessagesDialogsGetParams
            {
                Count = 200
            });
#pragma warning restore CS0618 // Тип или член устарел
            var curmessage = "";
            var state = false;
            for (var i = 0; i < 200; i++)
            {
                if (getDialogs.Messages[i].UserId == idsob)
                {
                    state = (bool)getDialogs.Messages[i].Out;
                    curmessage = getDialogs.Messages[i].Body;
                    break;
                }
            }

            if (curmessage.Length < 13)
            {
                return false;
            }

            if (curmessage.Substring(0, 13) == "<RSAKeyValue>" && state == false)
            {
                return true;
            }

            if (pr)
            {
                return false;
            }

            return false;
        }

        private void Send_Key(VkApi api, string pubkey, int idsob, bool pr)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Идет отправка публичного ключа..." + Environment.NewLine;
            }));            
            var random = new Random();
            var randid = random.Next(99999);
            api.Messages.Send(new MessagesSendParams
            {
                UserId = idsob,
                RandomId = randid,
                Message = pubkey
            });
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Ключ успешно отправлен!!!" + Environment.NewLine;
            }));
            if (pr)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Chat.Text += "Дожидаемся получения ключа..." + Environment.NewLine;
                    Chat.Text += "Можете сходить за кофе" + Environment.NewLine;
                }));
            }
        }

        private string Get_Key(VkApi api, int idsob)
        {
            string newkey;
            while (true)
            {
#pragma warning disable CS0618 // Тип или член устарел
                MessagesGetObject getDialogs = api.Messages.GetDialogs(new MessagesDialogsGetParams
                {
                    Count = 200
                });
#pragma warning restore CS0618 // Тип или член устарел
                Thread.Sleep(500);
                var curmessage = "";
                var state = false;
                int i;
                for (i = 0; i < 200; i++)
                {
                    if (getDialogs.Messages[i].UserId == idsob)
                    {
                        state = (bool)getDialogs.Messages[i].Out;
                        curmessage = getDialogs.Messages[i].Body;
                        break;
                    }
                }

                if (curmessage.Length < 13)
                {
                    continue;
                }

                if (curmessage.Substring(0, 13) == "<RSAKeyValue>" && state == false)
                {
                    newkey = curmessage;
                    break;
                }
            }

            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Chat.Text += "Ключ получен" + Environment.NewLine;
            }));            
            return newkey;
        }
    }
}
