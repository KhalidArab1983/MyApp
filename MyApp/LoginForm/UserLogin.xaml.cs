using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MyApp
{
    /// <summary>
    /// Interaktionslogik für UserLogin.xaml
    /// </summary>
    public partial class UserLogin : Window
    {
        public static UserLogin myLoginForm { get; private set; }

        static UserLogin()
        {
            myLoginForm = new UserLogin();
        }


        readonly MainWindow mainWindow = new();
        readonly string mainConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        //private readonly Image currImage = null;
        readonly UserRegister register = new();
        public UserLogin()
        {
            InitializeComponent();
        }

        private void BtnClickLogin_OnLogin(object sender, RoutedEventArgs e)
        {

            //Connection String

            MySqlConnection sqlConn = new(mainConn);

            if (TxBxUserEmail.Text == "")
            {
                ErrorTextBlock.Text = "*Email required! please enter your email.";
                TxBxUserEmail.Focus();
            }
            else if (TxBxUserPass.Password == "")
            {
                ErrorTextBlock.Text = "*Password required! please enter your Password.";
            }
            else if (TxBxUserPass.Password.Length > 30)
            {
                ErrorTextBlock.Text = "*Error! Wrong Password, try again.";
            }
            else if (!Regex.IsMatch(TxBxUserEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-"
                                                        + @"9]@[a-zA-Z0-9][\w\.-]*"
                                                        + @"[a-zA-Z0-9]\.[a-zA-Z][a-z"
                                                        + @"A-Z\.]*[a-zA-Z]$"))
            {
                ErrorTextBlock.Text = "*Invalid email! please enter a valid email.";
                TxBxUserEmail.Select(0, TxBxUserEmail.Text.Length);
                TxBxUserEmail.Focus();
            }
            else
            {
                try
                {
                    string userEmail = TxBxUserEmail.Text;
                    string userPass = TxBxUserPass.Password;
                    sqlConn.Open();
                    string query = "SELECT * FROM userregister WHERE email = '" + userEmail + "' AND password = '" + UserRegister.HashedPassword(userPass) + "'";
                    MySqlCommand command = new(query, sqlConn);
                    command.CommandType = CommandType.Text;
                    MySqlDataAdapter adapter = new();
                    adapter.SelectCommand = command;
                    DataSet ds = new();
                    adapter.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        mainWindow.WelcomeToMainWindow.Text = ds.Tables[0].Rows[0]["userName"].ToString();
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Sorry! Please enter existing email or password", "not exist", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    sqlConn.Close();
                }
                catch (Exception ex)
                {
                    ErrorTextBlock.Text = ex.Message;
                }

            }

        }


        //private void ShowImage()
        //{
        //    DataTable dt = ds.Tables[0];
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        byte[] blob = (byte[])row[1];
        //        MemoryStream mStream = new();
        //        mStream.Write(blob, 0, blob.Length);
        //        mStream.Position = 0;

        //        System.Drawing.Image img = System.Drawing.Image.FromStream(mStream);
        //        BitmapImage bitmap = new();
        //        bitmap.BeginInit();

        //        MemoryStream ms = new();
        //        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        bitmap.StreamSource = ms;
        //        bitmap.EndInit();
        //        //mainWindow.ImageBoxShow.Source = bitmap;
        //    }
        //}


        private void BtnClickRegister_OnLogin(object sender, RoutedEventArgs e)
        {
            register.Show();
            Close();
        }


        private void EyeOnPassHide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOnPassHide.Visibility = Visibility.Hidden;
                EyeOffPassVisible.Visibility = Visibility.Visible;

                PasswordUnmask.Visibility = Visibility.Visible;
                TxBxUserPass.Visibility = Visibility.Hidden;
                PasswordUnmask.Text = TxBxUserPass.Password;
            }
        }


        private void EyeOffPassVisible_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOffPassVisible.Visibility = Visibility.Hidden;
                EyeOnPassHide.Visibility = Visibility.Visible;

                PasswordUnmask.Visibility = Visibility.Hidden;
                TxBxUserPass.Visibility = Visibility.Visible;
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ForgetPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SendCodeForm sendForm = new();
            sendForm.Show();
            Close();
        }

        private void ChBoxSavePass_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.email = TxBxUserEmail.Text;
            Properties.Settings.Default.password = TxBxUserPass.Password;
            Properties.Settings.Default.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxBxUserEmail.Text = Properties.Settings.Default.email;
            TxBxUserPass.Password = Properties.Settings.Default.password;
        }

        MediaPlayer player = new MediaPlayer();
        private void BtnClickLogin_OnLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnClickRegister_OnLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
    }
}
