using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyApp
{
    /// <summary>
    /// Interaktionslogik für UserRegister.xaml
    /// </summary>
    public partial class UserRegister : Window
    {
        public static UserRegister myRegisterForm { get; private set; }

        static UserRegister()
        {
            myRegisterForm = new UserRegister();
        }

        readonly string mainConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        //readonly DataSet ds;
        private string imageName;

        private string strName;

        public UserRegister()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        //@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
        //+ @"([-a-<-z0-9!#$%&'*+/=?^_`{|}]|(?<!\.)\.)*)(?<°\.)"
        //+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$"))


#pragma warning disable SYSLIB0021 // Type or member is obsolete
        public static string HashedPassword(string value)
        {
            using HashAlgorithm hmaHash = new SHA256Managed();
            UTF8Encoding utf8 = new();
            byte[] data = hmaHash.ComputeHash(utf8.GetBytes(value));
            return Convert.ToBase64String(data);
        }


        private void BtnClickRegister_OnRegister(object sender, RoutedEventArgs e)
        {
            //Connection String

            MySqlConnection sqlConn = new(mainConn);

            if (TxBxUserName.Text == "" || TxBxUserEmail.Text == "" || 
                TxBxUserPass.Password == "" || TxBxUserPassConfirm.Password == "")
            {
                ErrorTextBlock.Text = "something is missing! please fill mandatory fields.";
            }
            else if (!Regex.IsMatch(TxBxUserEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-"
                                                        + @"9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]"
                                                        + @"\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                ErrorTextBlock.Text = "Invalid email! please enter a valid email.";
                TxBxUserEmail.Select(0, TxBxUserEmail.Text.Length);
                TxBxUserEmail.Focus();
            }

            else if (TxBxUserPass.Password.Length < 8)
            {
                ErrorTextBlock.Text = "Password must be more than 8 characters, and it's prefered to contains special characters.";
            }
            else if (TxBxUserPass.Password != TxBxUserPassConfirm.Password)
            {
                ErrorTextBlock.Text = "Error! Password does not confirmed.";
            }

            else
            {
                // Verify the existence of the account
                sqlConn.Open();
                bool exists = false;
                using (MySqlCommand cmd = new("select count(*) from userregister where email = @email", sqlConn))
                {
                    cmd.Parameters.AddWithValue("email", TxBxUserEmail.Text);
                    exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                if (exists)
                {
                    successTextBlock.Visibility = Visibility.Hidden;
                    ErrorTextBlock.Text = $"Account already exists, please try with another E-mail or click on LOGIN to sign in.";
                }
                else
                {
                    try
                    {
                        //Read a bitmap's contents in a stream
                        FileStream fs = new(imageName, FileMode.OpenOrCreate, FileAccess.Read);
                        byte[] rawData = new byte[fs.Length];
                        fs.Read(rawData, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        //Construct a SQL string and a connection object
                        string sql = "select * from userregister";
                        MySqlConnection conn = new();
                        conn.ConnectionString = mainConn;

                        //Open the connection
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        //Create a data adapter and data set
                        MySqlDataAdapter adapter = new(sql, conn);
                        MySqlCommandBuilder cmdBuilder = new(adapter);
                        DataSet ds = new("userregister");
                        adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                        //Fill the data adapter
                        adapter.Fill(ds, "userregister");

                        //Create a new row
                        DataRow row = ds.Tables["userregister"].NewRow();
                        row["userName"] = TxBxUserName.Text;
                        row["email"] = TxBxUserEmail.Text;
                        row["password"] = HashedPassword(TxBxUserPass.Password);
                        row["image"] = rawData;

                        //Add the row to the collection
                        ds.Tables["userregister"].Rows.Add(row);

                        //Save changes to the database
                        adapter.Update(ds, "userregister");

                        //Clean up connection
                        if (conn != null)
                        {
                            if (conn.State == ConnectionState.Open)
                                conn.Close();
                        }

                        ErrorTextBlock.Visibility = Visibility.Hidden;
                        successTextBlock.Text = $"{TxBxUserEmail.Text} has successfull registered.";

                        Clear();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, "Registration is faild");
                        successTextBlock.Visibility = Visibility.Hidden;
                        ErrorTextBlock.Text = "Registration is faild.\n" + ex.Message;
                    }
                }
                sqlConn.Close();
            }


        }


        void Clear()
        {
            TxBxUserName.Text = TxBxUserEmail.Text = TxBxUserPass.Password = TxBxUserPassConfirm.Password = PasswordUnmask.Text = PasswordConfirmUnmask.Text = "";
            ImageBox.Source = null;
        }

        private void BtnClickLogin_OnRegister(object sender, RoutedEventArgs e)
        {
            UserLogin login = new();
            login.Show();
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

        private void EyeOnPassConfirmHide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOnPassConfirmHide.Visibility = Visibility.Hidden;
                EyeOffPassConfirmVisible.Visibility = Visibility.Visible;

                PasswordConfirmUnmask.Visibility = Visibility.Visible;
                TxBxUserPassConfirm.Visibility = Visibility.Hidden;
                PasswordConfirmUnmask.Text = TxBxUserPassConfirm.Password;

            }
        }

        private void EyeOffPassConfirmVisible_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOffPassConfirmVisible.Visibility = Visibility.Hidden;
                EyeOnPassConfirmHide.Visibility = Visibility.Visible;

                PasswordConfirmUnmask.Visibility = Visibility.Hidden;
                TxBxUserPassConfirm.Visibility = Visibility.Visible;
            }

        }

        private void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void TextReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TxBxUserEmail.Text = TxBxUserName.Text = TxBxUserPass.Password = TxBxUserPassConfirm.Password = "";
                ErrorTextBlock.Text = successTextBlock.Text = "";
                ImageBox.Source = null;
            }
        }


        static string GeneratePass(int length)
        {
            const string validChars = @"abcdefghijklmnopqrstuvwxyzABCDEFGHI"
                   + @"JKLMNOPQRSTUVWXYZ1234567890+-§$%&/=?€~\µ|<>!*{[(@)]}";
            StringBuilder res = new();
            Random rand = new();
            while (0 < length--)
            {
                res.Append(validChars[rand.Next(validChars.Length)]);
            }
            return res.ToString();
        }

        private void BtnPassGenerate_Click(object sender, RoutedEventArgs e)
        {
            int length = 10;
            string pass = GeneratePass(length);
            TxBxUserPass.Password = pass;
            TxBxUserPassConfirm.Password = pass;
            PasswordUnmask.Text = pass;
            PasswordConfirmUnmask.Text = pass;
        }





        private void SelectImageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openImage = new OpenFileDialog();
            openImage.Filter = "JPG files|*.jpg; *.jpeg|PNG files|*.png|GIF files|*.gif|BMP files|*.bmp|All Files|*.*";
            openImage.Multiselect = false;
            openImage.DefaultExt = ".jpg";

            Nullable<bool> dialogOK = openImage.ShowDialog();
            if (dialogOK == true)
            {
                strName = openImage.SafeFileName;
                imageName = openImage.FileName;
                ImageSourceConverter isc = new();
                ImageBox.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
            }
        }

        MediaPlayer player = new MediaPlayer();
        private void BtnClickLogin_OnRegister_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnClickRegister_OnRegister_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnPassGenerate_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
    }
}
