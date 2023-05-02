using com.sun.tools.javac.comp;
using Microsoft.AspNetCore.Html;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyApp
{
    /// <summary>
    /// Interaktionslogik für SendCodeForm.xaml
    /// </summary>
    public partial class SendCodeForm : Window
    {
        string randomCode;

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string? to;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public SendCodeForm()
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

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            //Connection String
            string mainConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            MySqlConnection sqlConn = new(mainConn);
            sqlConn.Open();
            MySqlCommand cmd = new("select * from userregister where email = '" + TxBxVerifyEmail.Text + "'", sqlConn);
            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows == true)
            {
                dr.Read();
                Random rand = new();
                randomCode = rand.Next(999999).ToString();
                SmtpClient client = new()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "rojdaf8@gmail.com",
                        Password = "covyppaonfovaywk"
                    }
                };

                string htmlText = "<html>" +
                                    "<head>" +
                                        "<title>Password Reset Request</title>" +
                                    "</head>" +
                                    "<body>" +
                                        $"<h3 style=text-align:center>Hallo <span style=color:red>{TxBxVerifyEmail.Text}</span></h3>" +
                                         "<p style=text-align:center;font-size:20px>A password reset has been requested for your account.</p>" +
                                         "<p style=text-align:center;font-size:20px>If you did not request this change, you can safely ignore this email.</p>" +
                                         "<p style=text-align:center;font-size:20px> Please enter the following code in the specified place and click on “Verify”" +
                                         $"<p style=text-align:center;font-size:20px>Your reset code is :</p>" +
                                         $"<p style=text-align:center;color:blue;font-size:30px;border:black solid 2px;>{randomCode}</p>" +
                                         "<img src='https://s7280.pcdn.co/wp-content/uploads/2016/06/database-blue.png'>" +
                                    "</body>" +
                                  "</html>";
                                    
                
                MailAddress FromEmail = new("rojdaf8@gmail.com", "noReply");
                MailAddress ToEmail = new(TxBxVerifyEmail.Text);
                MailMessage Message = new()
                {
                    From = FromEmail,
                    Subject = "Password reset",
                    Body = htmlText,
                    IsBodyHtml = true
                };

                Message.To.Add(ToEmail);
                client.SendCompleted += Client_SendCompleted;
                client.SendMailAsync(Message);
            }
            else
            {
                MessageBox.Show("email address not found!");

            }
            sqlConn.Close();

        }

        //private string FetchUserName()
        //{
        //    //Connection String
        //    string mainConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        //    MySqlConnection sqlConn = new MySqlConnection(mainConn);
        //    sqlConn.Open();
        //    string fetchUserName = "select userName from userregister where email = '" + TxBxVerifyEmail.Text + "'";
        //    MySqlCommand fetchComm = new MySqlCommand(fetchUserName, sqlConn);
        //    MySqlDataReader drFetch = fetchComm.ExecuteReader();
        //    if(drFetch.HasRows == true)
        //    {
        //        drFetch.Read();

        //    }
        //    return fetchUserName;
        //}
        private void Client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Something went wrong \n" + e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Sent successfully" + "\n" + "Please check your spam too!", "Done", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            if (randomCode == (TxBxVerifyCode.Text).ToString())
            {
                to = TxBxVerifyEmail.Text;
                ResetPassViaEmail resetForm = new();
                resetForm.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Wrong Code" + "\n" + "please try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserLogin login = new();
            login.Show();
            Close();
        }

        MediaPlayer player = new MediaPlayer();
        private void BtnSendCode_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnVerify_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
    }
}
