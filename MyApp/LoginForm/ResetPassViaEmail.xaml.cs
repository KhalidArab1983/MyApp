using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MyApp
{
    /// <summary>
    /// Interaktionslogik für ResetPassViaEmail.xaml
    /// </summary>
    public partial class ResetPassViaEmail : Window
    {
        readonly string email = SendCodeForm.to;
        public ResetPassViaEmail()
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

        private void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void BackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserLogin login = new();
            login.Show();
            Close();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (TxBxUserNewPass.Password == TxBxUserConfirmPass.Password)
            {
                //Connection String
                string mainConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
                MySqlConnection sqlConn = new(mainConn);
                string updateQuery = "update userregister set password = '" + UserRegister.HashedPassword(TxBxUserNewPass.Password) + "' where email = '" + email + "'";
                MySqlCommand cmd = new(updateQuery, sqlConn);

                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();

                TxBxUserNewPass.Password = TxBxUserConfirmPass.Password = "";
                MessageBox.Show("Password has been changed successfully", "Done");
            }
            else
            {
                MessageBox.Show("Sorry! could not update the password", "Error");
            }
        }

        private void EyeOnNewPassHide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                EyeOnNewPassHide.Visibility = Visibility.Hidden;
                EyeOffNewPassVisible.Visibility = Visibility.Visible;

                NewPasswordUnmask.Visibility = Visibility.Visible;
                TxBxUserNewPass.Visibility = Visibility.Hidden;
                NewPasswordUnmask.Text = TxBxUserNewPass.Password;

            }
        }

        private void EyeOffNewPassVisible_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOffNewPassVisible.Visibility = Visibility.Hidden;
                EyeOnNewPassHide.Visibility = Visibility.Visible;

                NewPasswordUnmask.Visibility = Visibility.Hidden;
                TxBxUserNewPass.Visibility = Visibility.Visible;
            }
        }

        private void EyeOnConfirmPassHide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOnConfirmPassHide.Visibility = Visibility.Hidden;
                EyeOffConfirmPassVisible.Visibility = Visibility.Visible;

                ConfirmPasswordUnmask.Visibility = Visibility.Visible;
                TxBxUserConfirmPass.Visibility = Visibility.Hidden;
                ConfirmPasswordUnmask.Text = TxBxUserConfirmPass.Password;

            }
        }

        private void EyeOffConfirmPassVisible_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                EyeOffConfirmPassVisible.Visibility = Visibility.Hidden;
                EyeOnConfirmPassHide.Visibility = Visibility.Visible;

                ConfirmPasswordUnmask.Visibility = Visibility.Hidden;
                TxBxUserConfirmPass.Visibility = Visibility.Visible;
            }
        }

        MediaPlayer player = new MediaPlayer();
        private void BtnReset_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
    }
}
