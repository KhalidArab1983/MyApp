using jdk.nashorn.@internal.runtime;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using MySql.Data.MySqlClient;
using sun.tools.javac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static com.sun.tools.javac.comp.Annotate;
//using File = System.IO.File;

namespace MyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly List<Task> TaskList = new();
        public ObservableCollection<AddRowClass> NewRows { get; set; }
        //public ObservableCollection<TransferedTable> TableNameRowCount { get; set; }

        BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            TxBxSourceServer.FilterMode = TxBxTargetServer.FilterMode = TxBxSourceUserName.FilterMode = TxBxTargetUserName.FilterMode = TxBxDBName.FilterMode = TxBxSourcePort.FilterMode = TxBxTargetPort.FilterMode = AutoCompleteFilterMode.Contains;
            TxBxSourceServer.ItemsSource = new string[] { "localhost", @"SO529\SQLEXPRESS", "SO529", "mytestsqlserver.database.windows.net", "sql9.freesqldatabase.com" };
            TxBxTargetServer.ItemsSource = new string[] { "localhost", @"SO529\SQLEXPRESS", "SO529", "mytestsqlserver.database.windows.net" };
            TxBxSourceUserName.ItemsSource = new string[] { "sotectesting", "root", "sql9569369" };
            TxBxTargetUserName.ItemsSource = new string[] { "sotectesting", "root" };
            //TxBxDBPath.ItemsSource = new string[] { @"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA", @"C:\NewTestDB" };
            TxBxTargetPort.ItemsSource = new string[] {"3306", "3308", "3309" };
            TxBxSourcePort.ItemsSource = new string[] { "3306", "3308", "3309" };
            TxBxDBName.ItemsSource = new string[] {"mitarbeiter", "myapp", "university" };

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;


            NewRows = new ObservableCollection<AddRowClass>() { };
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            NewRows.CollectionChanged += new NotifyCollectionChangedEventHandler(NewRows_CollectionChanged);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            this.DataContext = NewRows;

        }


        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            ProgressBarDone.Value = e.ProgressPercentage;
            ProgressBarText.Text = ProgressBarDone.Value.ToString() + "%";
        }

        //void TableNameRowCount_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{

        //}


        void NewRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

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

        // Button to attach databases to local server
        private void BtnAttach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Server myServer = new(TxBxTargetServer.Text);
                Database dbAttach = myServer.Databases[TxBxDBName.Text];
                string dbPath = TxBxDBPath.Text + @"\" + TxBxDBName.Text + ".mdf";
                string dbLogPath = TxBxDBPath.Text + @"\" + TxBxDBName.Text + "_log.ldf";

                StringCollection Paths = new()
                {
                    dbPath,
                    dbLogPath
                };
                myServer.AttachDatabase(TxBxDBName.Text, Paths);

                ImageRightGif.Visibility = Visibility.Visible;
                successTextBlock.Text = "Database has been successfully attached.";
                SuccessDelay();
                transferedTextBlock.Text = "";
                ErrorTextBlock.Text = "";
                ImageErrorGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {

                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // Button to detach databases from local server
        private void BtnDetach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CoBoxSourceDBType.Text == "LOCAL")
                {
                    Server myServer = new(TxBxSourceServer.Text);
                    myServer.DetachDatabase(CoBoxDB.Text, true);
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "Database has been successfully detached.";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please select a database type as LOCAL!";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }


        private void BtnCheckResult_Click(object sender, RoutedEventArgs e)
        {
            //int tableCountTarget = 0;
            //int rowsNbrTarget = 0;

            //string targetServer = TxBxTargetServer.Text;
            //string targetUserName = TxBxTargetUserName.Text;
            //string targetPass = TxBxTargetPassword.Password;
            //string targetPort = TxBxTargetPort.Text;
            //string targetDbName = TxBxDBName.Text;
            //string connTargetServer = "Server = '" + targetServer + "'; user = '" + targetUserName + "'; Password = '" + targetPass + "'; database= '" + targetDbName + "'; port= '" + targetPort + "'";
            //using MySqlConnection connResult = new(connTargetServer);
            //using MySqlCommand tableCommTarget = new();
            //tableCommTarget.Connection = connResult;
            //connResult.Open();

            ////tableCommTarget.CommandText = $"Use {dbName3}";
            ////tableCommTarget.ExecuteNonQuery();

            //tableCommTarget.CommandText = $"SELECT TABLE_NAME, SUM(TABLE_ROWS) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{targetDbName}' GROUP BY TABLE_NAME;";
            //tableCommTarget.ExecuteNonQuery();

            //DataTable myTableTarget = new DataTable();
            //myTableTarget.Columns.Add("TableNumberTarget", typeof(string));
            //myTableTarget.Columns.Add("TableNameTarget", typeof(string));
            //myTableTarget.Columns.Add("RowsCountTarget", typeof(string));

            //MySqlDataReader tblReaderTarget = tableCommTarget.ExecuteReader();
            //while (tblReaderTarget.Read())
            //{
            //    myTableTarget.Rows.Add(myTableTarget.Rows.Count + 1, tblReaderTarget["TABLE_NAME"], tblReaderTarget["SUM(TABLE_ROWS)"]);
            //    DataGridTableTarget.ItemsSource = myTableTarget.DefaultView;
            //    ++tableCountTarget;
            //}
            //foreach (DataRowView rowTarget in DataGridTableTarget.ItemsSource)
            //{
            //    rowsNbrTarget += Convert.ToInt32(rowTarget["RowsCountTarget"]);
            //}


            //TotalTablesCountLabelTarget.Content = tableCountTarget.ToString();
            //TotalRowsCountLabelTarget.Content = rowsNbrTarget.ToString();

            //connResult.Close();

        }
 

        // Button to transfer databases directly from the source to the target
        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxBxTargetServer.Text == "" || TxBxTargetUserName.Text == "")
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please fill mandatory fields in TARGET SERVER whose borders become red when the mouse is hovered over the Transfer button";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else if (TxBxSourceServer.Text == "" || TxBxSourceUserName.Text == "")
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please fill mandatory fields in SOURCE SERVER whose borders become red when the mouse is hovered over the Transfer button";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else if (CoBoxDB.Text == "" || CoBoxDB.Text == "Select a Database . . .")
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please select a database.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else if (TxBxDBName.Text == "" || TxBxDBPath.Text == "")
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please fill mandatory fields in DB FILE PATH and DB NAME";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else if (CoBoxTargetDBType.Text == "" || CoBoxTargetDBType.Text == "Select a database type...")
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Please select a database type in Target Server.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    string sourceServer = TxBxSourceServer.Text;
                    string sourceUserName = TxBxSourceUserName.Text;
                    string sourcePass = TxBxSourcePassword.Password;
                    string sourcePort = TxBxSourcePort.Text;
                    string sourceDbName = CoBoxDB.Text;
                    string targetFile = TxBxDBPath.Text + @"\" + @"Backup_" + sourceDbName + ".sql";
                    string connSourceServer = "Server = '" + sourceServer + "'; user = '" + sourceUserName + "'; Password = '" + sourcePass + "'; database= '" + sourceDbName + "'; port='" + sourcePort + "'";
                    using MySqlConnection connSource = new(connSourceServer);
                    using MySqlCommand cmdSource = new();
                    using MySqlBackup mb = new(cmdSource);
                    cmdSource.Connection = connSource;
                    connSource.Open();
                    mb.ExportToFile(targetFile);
                    connSource.Close();

                    string serverCreateDB = TxBxTargetServer.Text;
                    string userNameCreateDB = TxBxTargetUserName.Text;
                    string passCreateDB = TxBxTargetPassword.Password;
                    string portCreateDB = TxBxTargetPort.Text;
                    string connectionCreateDB = "host='" + serverCreateDB + "'; user='" + userNameCreateDB + "'; Password='" + passCreateDB + "'; port= '" + portCreateDB + "'";
                    MySqlConnection connCreateDB = new(connectionCreateDB);
                    connCreateDB.Open();
                    MySqlCommand cmdCreateDB = new($"create database if not exists {TxBxDBName.Text}", connCreateDB);
                    cmdCreateDB.ExecuteNonQuery();
                    connCreateDB.Close();

                    string targetServer = TxBxTargetServer.Text;
                    string targetUserName = TxBxTargetUserName.Text;
                    string targetPass = TxBxTargetPassword.Password;
                    string targetPort = TxBxTargetPort.Text;
                    string targetDbName = TxBxDBName.Text;
                    string sourceFile = TxBxDBPath.Text + @"\" + @"Backup_" + targetDbName + ".sql";
                    string connTargetServer = "Server = '" + targetServer + "'; user = '" + targetUserName + "'; Password = '" + targetPass + "'; database= '" + targetDbName + "'; port= '" + targetPort + "'";
                    using MySqlConnection connTarget = new(connTargetServer);
                    using MySqlCommand cmdTarget = new();
                    using MySqlBackup mb2 = new(cmdTarget);
                    cmdTarget.Connection = connTarget;
                    connTarget.Open();

                    //cmd2.CommandText = "SET GLOBAL max_allowed_packet=429416777216;";
                    //cmd2.ExecuteNonQuery();
                    mb2.ImportFromFile(sourceFile);
                    connTarget.Close();


                    int rowsNbrSource = 0;
                    int rowsNbrTarget = 0;
                    int lostRows = 0;

                    int tableCountSource = 0;
                    int tableCountTarget = 0;
                    int lostTable = 0;

                    ////////////// Show result of Source Server Database ///////////////
                    using MySqlConnection tableConnSource = new(connSourceServer);
                    using MySqlCommand tableCommSource = new();
                    tableCommSource.Connection = tableConnSource;
                    tableConnSource.Open();

                    tableCommSource.CommandText = $"SELECT TABLE_NAME, SUM(TABLE_ROWS) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{sourceDbName}' GROUP BY TABLE_NAME;";
                    tableCommSource.ExecuteNonQuery();

                    DataTable myTableSource = new DataTable();
                    myTableSource.Columns.Add("TableNumber", typeof(string));
                    myTableSource.Columns.Add("TableName", typeof(string));
                    myTableSource.Columns.Add("RowsCount", typeof(string));

                    MySqlDataReader tblReaderSource = tableCommSource.ExecuteReader();
                    while (tblReaderSource.Read())
                    {
                        myTableSource.Rows.Add(myTableSource.Rows.Count + 1, tblReaderSource["TABLE_NAME"], tblReaderSource["SUM(TABLE_ROWS)"]);
                        DataGridTable.ItemsSource = myTableSource.DefaultView;
                        ++tableCountSource;
                    }
                    foreach (DataRowView row in DataGridTable.ItemsSource)
                    {
                        rowsNbrSource += Convert.ToInt32(row["RowsCount"]);
                    }
                    TotalTablesCountLabel.Content = tableCountSource.ToString();
                    TotalRowsCountLabel.Content = rowsNbrSource.ToString();
                    tableConnSource.Close();


                    //if (conn.State == ConnectionState.Closed && tableConn.State == ConnectionState.Closed)
                    //{
                    //    DateTime StartTime = DateTime.Now;
                    //    TimeSpan ts = DateTime.Now.Subtract(StartTime);
                    //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    //    Thread.Sleep(Convert.ToInt32(elapsedTime));
                    //}


                    //Hier die Funktion einfügen deren Zeit gemessen werden soll
                    watch.Stop();
                    long totalMilliseconds = watch.ElapsedMilliseconds;
                    //MessageBox.Show($"Time spent is: {totalMilliseconds} ms.");
                    //Thread.Sleep(Convert.ToInt32(totalMilliseconds));
                    //worker.RunWorkerAsync();

                    ////////////// Show result of Target Server Database ///////////////
                    string server3 = TxBxTargetServer.Text;
                    string userName3 = TxBxTargetUserName.Text;
                    string pass3 = TxBxTargetPassword.Password;
                    string port3 = TxBxTargetPort.Text;
                    string dbName3 = TxBxDBName.Text;
                    string connString3 = "Server = '" + server3 + "'; user = '" + userName3 + "'; Password = '" + pass3 + "'; database= '" + dbName3 + "'; port= '" + port3 + "'";
                    using MySqlConnection tableConnTarget = new(connString3);
                    using MySqlCommand tableCommTarget = new();
                    tableCommTarget.Connection = tableConnTarget;
                    tableConnTarget.Open();

                    //tableCommTarget.CommandText = $"Use {dbName3}";
                    //tableCommTarget.ExecuteNonQuery();

                    tableCommTarget.CommandText = $"SELECT TABLE_NAME, SUM(TABLE_ROWS) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{dbName3}' GROUP BY TABLE_NAME;";
                    tableCommTarget.ExecuteNonQuery();

                    DataTable myTableTarget = new DataTable();
                    myTableTarget.Columns.Add("TableNumberTarget", typeof(string));
                    myTableTarget.Columns.Add("TableNameTarget", typeof(string));
                    myTableTarget.Columns.Add("RowsCountTarget", typeof(string));

                    MySqlDataReader tblReaderTarget = tableCommTarget.ExecuteReader();
                    while (tblReaderTarget.Read())
                    {
                        myTableTarget.Rows.Add(myTableTarget.Rows.Count + 1, tblReaderTarget["TABLE_NAME"], tblReaderTarget["SUM(TABLE_ROWS)"]);
                        DataGridTableTarget.ItemsSource = myTableTarget.DefaultView;
                        ++tableCountTarget;
                    }
                    foreach (DataRowView rowTarget in DataGridTableTarget.ItemsSource)
                    {
                        rowsNbrTarget += Convert.ToInt32(rowTarget["RowsCountTarget"]);
                    }


                    TotalTablesCountLabelTarget.Content = tableCountTarget.ToString();
                    TotalRowsCountLabelTarget.Content = rowsNbrTarget.ToString();


                    if (tableCountTarget < tableCountSource || rowsNbrTarget < rowsNbrSource)
                    {
                        lostTable = tableCountSource - tableCountTarget;
                        lostRows = rowsNbrSource - rowsNbrTarget;
                        ErrorTextBlock.Text = $"{lostTable} table/s and {lostRows} data row/s have been lost, Please try again";
                        ImageErrorGif.Visibility = Visibility.Visible;
                        //ErrorDelay();
                        transferedTextBlock.Text = "";
                        successTextBlock.Text = "";
                        ImageRightGif.Visibility = Visibility.Hidden;
                        infoTextBlock.Text = "";
                        ImageInfoGif.Visibility = Visibility.Hidden;
                        ArrowRed.Visibility = Visibility.Visible;
                        ArrowGrenn.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        transferedTextBlock.Text = $"{rowsNbrSource} Data Rows and {tableCountSource} Tables have been successfully transfered.";
                        ImageRightGif.Visibility = Visibility.Visible;
                        //SuccessDelay();
                        successTextBlock.Text = "";
                        ErrorTextBlock.Text = "";
                        infoTextBlock.Text = "";
                        ImageErrorGif.Visibility = Visibility.Hidden;
                        ImageInfoGif.Visibility = Visibility.Hidden;
                        ArrowGrenn.Visibility = Visibility.Visible;
                        ArrowRed.Visibility = Visibility.Hidden;
                    }

                    int newIndex = Tab_Control.SelectedIndex + 3;
                    if (newIndex >= Tab_Control.Items.Count)
                        newIndex = 0;
                    Tab_Control.SelectedIndex = newIndex;

                    tableConnTarget.Close();
                }
            }
            catch (Exception ex)
            {

                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                //ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }

        }



        public class TransferedTable 
        {
            public string? TableNumber { get; set; }
            public string? TableName { get; set; }
            public int? RowsCount { get; set; }
        }
        
        public class TransferedTableTarget
        {
            public string? TableNumberTarget { get; set; }
            public string? TableNameTarget { get; set; }
            public int? RowsCountTarget { get; set; }
        }
        // button to delete databases
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CoBoxDB.Text == "" || CoBoxDB.Text == "Select a Database . . .")
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Please select a database.";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
            else
            {
                switch (System.Windows.MessageBox.Show("Do you really want to permanently delete the database? You cannot undo this action", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.No:
                        e.Handled = true;
                        break;

                    case MessageBoxResult.Yes:
                        DeleteDB();
                        break;
                }
            }

        }

        // Button to execute the method DbExportToFile for MySql or copy databases for local
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (CoBoxSourceDBType.Text == "MySQL")
            {
                DbExportToFile();
            }
            else if (CoBoxSourceDBType.Text == "SQL")
            {
                //ExportBacpac();
            }
            else if (CoBoxSourceDBType.Text == "LOCAL")
            {
                try
                {
                    Server myServer = new(TxBxSourceServer.Text);
                    if (CoBoxDB.Text == "" || CoBoxDB.Text == "Select a Database . . .")
                    {
                        ImageErrorGif.Visibility = Visibility.Visible;
                        ErrorTextBlock.Text = "Please select a database.";
                        ErrorDelay();
                        transferedTextBlock.Text = "";
                        successTextBlock.Text = "";
                        ImageRightGif.Visibility = Visibility.Hidden;
                        infoTextBlock.Text = "";
                        ImageInfoGif.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Database db = myServer.Databases[CoBoxDB.Text];
                        myServer.Databases[CoBoxDB.Text].SetOffline();
                        string dbPath = db.PrimaryFilePath + @"\" + CoBoxDB.Text + ".mdf";
                        string dbLogPath = db.PrimaryFilePath + @"\" + CoBoxDB.Text + "_log.ldf";

                        string dbDest = TxBxDBPath.Text + @"\" + CoBoxDB.Text + ".mdf";
                        string dbLogDest = TxBxDBPath.Text + @"\" + CoBoxDB.Text + "_log.ldf";
                        File.Copy(dbPath, dbDest);
                        File.Copy(dbLogPath, dbLogDest);
                        myServer.Databases[CoBoxDB.Text].SetOnline();

                        ImageRightGif.Visibility = Visibility.Visible;
                        successTextBlock.Text = "Database has been copied successfully.";
                        SuccessDelay();
                        transferedTextBlock.Text = "";
                        ErrorTextBlock.Text = "";
                        ImageErrorGif.Visibility = Visibility.Hidden;
                        infoTextBlock.Text = "";
                        ImageInfoGif.Visibility = Visibility.Hidden;
                    }

                }
                catch (Exception ex)
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = ex.Message;
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Please select a database type!";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // Button to execute the method TargetServerConnMySql(create a new database) and DbImportFromFile
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            if (CoBoxTargetDBType.Text == "MySQL")
            {
                TargetServerConnMySql();
                DbImportFromFile();

                //string server = TxBxTargetServer.Text;
                //string userName = TxBxTargetUserName.Text;
                //string pass = TxBxTargetPassword.Password;
                //string dbName = TxBxDBName.Text;
                //string port = TxBxTargetPort.Text;
                //string connString = "Server = '" + server + "'; user = '" + userName + "'; Password = '" + pass + "'; database= '" + dbName + "'; port= '" + port + "'";
                //using MySqlConnection conn = new(connString);
                //using MySqlCommand cmd = new();

                //cmd.Connection = conn;
                //conn.Open();

                //cmd.CommandText = "USE " + dbName;
                //cmd.ExecuteNonQuery();

                //cmd.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = {CoBoxDB.Text};";
                //long tableCount = 0;
                
                //tableCount = (Int64)cmd.ExecuteScalar();

                //MessageBox.Show(tableCount.ToString());
                //conn.Close();
            }
            else if (CoBoxTargetDBType.Text == "SQL")
            {
                TargetServerConnSql();
                DbImportFromFile();
            }
            else
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Please select a database type!";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }

        }

        // Button to fetch databases from different servers
        private void BtnGetDB_Click(object sender, RoutedEventArgs e)
        {
            if (CoBoxSourceDBType.Text == "SQL")
            {
                SourceServerConnSql();
            }
            else if (CoBoxSourceDBType.Text == "MySQL")
            {
                SourceServerConnMySql();
            }
            else if (CoBoxSourceDBType.Text == "LOCAL")
            {
                DbFetchLocal();
            }
            else
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Please select a database type in Source Server.";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //NewPasswordForm resetPass = new();
            //resetPass.Visibility = Visibility.Visible;
        }

        //////////// Methods ///////////


        // Delay to disappear success text hint
        private async void SuccessDelay()
        {
            TaskList.Add(Task.Delay(7_000));
            await Task.WhenAll(TaskList);
            if (TaskList.TrueForAll(t => t.IsCompleted))
            {
                TaskList.RemoveAll(t => t.IsCompleted);
                successTextBlock.Text = string.Empty;
                ImageRightGif.Visibility = Visibility.Hidden;
                transferedTextBlock.Text = string.Empty;
            }
        }

        // Delay to disappear error text hint
        private async void ErrorDelay()
        {
            TaskList.Add(Task.Delay(15_000));
            await Task.WhenAll(TaskList);
            if (TaskList.TrueForAll(t => t.IsCompleted))
            {
                TaskList.RemoveAll(t => t.IsCompleted);
                ErrorTextBlock.Text = string.Empty;
                ImageErrorGif.Visibility = Visibility.Hidden;
            }
        }

        // Export databases from server to local disk
        private void DbExportToFile()
        {
            try
            {
                string server = TxBxSourceServer.Text;
                string userName = TxBxSourceUserName.Text;
                string pass = TxBxSourcePassword.Password;
                string port = TxBxSourcePort.Text;
                string dbName = CoBoxDB.Text;
                string connString = "";
                string targetFile = TxBxDBPath.Text + @"\" + @"Backup_" + dbName + ".sql";
                if (CoBoxSourceDBType.Text == "MySQL")
                {
                    connString = "Server = '" + server + "'; user = '" + userName + "'; Password = '" + pass + "'; database= '" + dbName + "'; port='" + port + "'";
                }
                else if (CoBoxSourceDBType.Text == "SQL")
                {
                    connString = "Server = '" + server + "'; user = '" + userName + "'; Password = '" + pass + "'; database= '" + dbName + "'";
                }
                using MySqlConnection conn = new(connString);
                using MySqlCommand cmd = new();
                using MySqlBackup mb = new(cmd);
                if (TxBxDBPath.Text == "")
                {
                    ErrorTextBlock.Text = "Please enter a path to export the file to.";
                    TxBxDBPath.BorderBrush = Brushes.Yellow;
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorDelay();
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    transferedTextBlock.Text = "";
                }
                else if (CoBoxDB.Text == "" || CoBoxDB.Text == "Select a Database . . .")
                {
                    ErrorTextBlock.Text = "Please select a database from databases.";
                    CoBoxDB.Foreground = Brushes.Yellow;
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorDelay();
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    transferedTextBlock.Text = "";
                }
                else
                {
                    cmd.Connection = conn;
                    conn.Open();
                    mb.ExportToFile(targetFile);
                    conn.Close();
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "Database has been successfully exported";
                    SuccessDelay();
                    ErrorTextBlock.Text = "";
                    infoTextBlock.Text = "";
                    transferedTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    CoBoxDB.Foreground = Brushes.Black;
                    TxBxDBPath.BorderBrush = Brushes.Gray;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }

        }

        // Import databases from local disk to server
        private void DbImportFromFile()
        {
            try
            {
                string server = TxBxTargetServer.Text;
                string userName = TxBxTargetUserName.Text;
                string pass = TxBxTargetPassword.Password;
                string dbName = TxBxDBName.Text;
                string port = TxBxTargetPort.Text;
                string sourceFile = TxBxDBPath.Text + @"\" + @"Backup_" + dbName + ".sql";
                string connString = "Server = '" + server + "'; user = '" + userName + "'; Password = '" + pass + "'; database= '" + dbName + "'; port= '" + port + "'";
                using MySqlConnection conn = new(connString);
                using MySqlCommand cmd = new();
                using MySqlBackup mb = new(cmd);
                if (TxBxDBPath.Text == "")
                {
                    ErrorTextBlock.Text = "Please select a path to import the file from.";
                    TxBxDBPath.BorderBrush = Brushes.Yellow;
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else if (TxBxDBName.Text == "")
                {
                    ErrorTextBlock.Text = "Please type the name of the database.";
                    TxBxDBName.BorderBrush = Brushes.Yellow;
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else
                {
                    cmd.Connection = conn;
                    conn.Open();
                    //cmd.CommandText = "SET GLOBAL max_allowed_packet=True;";
                    //cmd.ExecuteNonQuery();
                    mb.ImportFromFile(sourceFile);

                    //cmd.CommandText = $"USE {dbName}";
                    //cmd.ExecuteNonQuery();
                    //cmd.CommandText = $"SELECT COUNT(*) FROM userregister";
                    //successTextBlock.Text = "The rows have been imported is: " + cmd.ExecuteScalar().ToString() + " Rows";
                    conn.Close();
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = $"Database has been successfully imported.";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    TxBxDBName.BorderBrush = Brushes.Gray;
                    TxBxDBPath.BorderBrush = Brushes.Gray;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }

        }

        // Fetch databases from local server
        private void DbFetchLocal()
        {
            try
            {
                Server myServer = new(TxBxSourceServer.Text);
                DatabaseCollection dbCol = myServer.Databases;
                CoBoxDB.Items.Clear();
                CoBoxDB.Items.Add("Select a Database . . .");
                CoBoxDB.Items.Add(new Separator());
                //ProgressBar();
                for (int j = 0; j < dbCol.Count; j++)
                {
                    CoBoxDB.Items.Add(dbCol[j].Name);
                    CoBoxDB.Items.Add(new Separator());
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "The databases have been fetched successfully";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;

                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }

        }

        // Source Server Connection for SQL server
        private void SourceServerConnSql()
        {
            try
            {
                string server = TxBxSourceServer.Text;
                string userName = TxBxSourceUserName.Text;
                string pass = TxBxSourcePassword.Password;
                string connectionString = "Server='" + server + "'; user='" + userName + "'; Password='" + pass + "'; Connection Timeout=30;";
                SqlConnection conn = new(connectionString);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "Database has been successfully connected and fetched.";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    //StackPanelDB.Visibility = Visibility.Visible;

                    string query = "select name from master.sys.databases;";
                    SqlCommand comm = new(query, conn);
                    SqlDataReader dr = comm.ExecuteReader();

                    CoBoxDB.Items.Clear();
                    CoBoxDB.Items.Add("Select a Database . . .");
                    CoBoxDB.Items.Add(new Separator());
                    while (dr.Read())
                    {
                        string databases = dr.GetString(0);
                        CoBoxDB.Items.Add(databases);
                        CoBoxDB.Items.Add(new Separator());
                    }
                }
                else
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Could not connect to the database.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // Source Server Connection for MySQL server
        public void SourceServerConnMySql()
        {
            string server = TxBxSourceServer.Text;
            string userName = TxBxSourceUserName.Text;
            string pass = TxBxSourcePassword.Password;
            string port = TxBxSourcePort.Text;
            string connectionString = "host='" + server + "'; user='" + userName + "'; Password='" + pass + "'; port= '" + port + "'; Connection Timeout=30;";
            try
            {
                MySqlConnection conn = new (connectionString);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "Database has been successfully connected and fetched.";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                    //StackPanelDB.Visibility = Visibility.Visible;

                    string query = "show databases;";
                    using MySqlCommand comm = new(query, conn);
                    using MySqlDataReader dr = comm.ExecuteReader();

                    CoBoxDB.Items.Clear();
                    CoBoxDB.Items.Add("Select a Database . . .");
                    CoBoxDB.Items.Add(new Separator());
                    //ProgressBar();
                    while (dr.Read())
                    {
                        string databases = dr.GetString(0);
                        CoBoxDB.Items.Add(databases);
                        CoBoxDB.Items.Add(new Separator());
                        
                    }
                    
                }
                else
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Could not connect to the database.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // Target Server Connection for Sql server
        private void TargetServerConnSql()
        {
            string server = TxBxTargetServer.Text;
            string userName = TxBxTargetUserName.Text;
            string pass = TxBxTargetPassword.Password;
            string connectionString = "Server='" + server + "'; user='" + userName + "';Password='" + pass + "';";
            try
            {
                SqlConnection conn = new(connectionString);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    ImageRightGif.Visibility = Visibility.Visible;
                    successTextBlock.Text = "Database has been successfully connected.";
                    SuccessDelay();
                    transferedTextBlock.Text = "";
                    ErrorTextBlock.Text = "";
                    ImageErrorGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
                else
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Could not connect to the database.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // Target Server Connection for MySql server
        private void TargetServerConnMySql()
        {
            string server = TxBxTargetServer.Text;
            string userName = TxBxTargetUserName.Text;
            string pass = TxBxTargetPassword.Password;
            string port = TxBxTargetPort.Text;
            string dbName = TxBxDBName.Text;
            string connectionString = "host='" + server + "'; user='" + userName + "'; Password='" + pass + "'; port= '" + port + "'; Connection Timeout=30;";

            try
            {
                MySqlConnection conn = new (connectionString);

                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    MySqlCommand cmd = new($"create database if not exists {dbName}", conn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = "Could not create the database.";
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        // To delete databases
        private void DeleteDB()
        {
            if (CoBoxSourceDBType.Text == "MySQL")
            {
                string server = TxBxSourceServer.Text;
                string userName = TxBxSourceUserName.Text;
                string pass = TxBxSourcePassword.Password;
                string connectionString = "host='" + server + "'; user='" + userName + "'; Password='" + pass + "'; Connection Timeout=30;";
                try
                {
                    MySqlConnection conn = new(connectionString);
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        if (CoBoxDB.Text == "" || CoBoxDB.Text == "Select a Database . . .")
                        {
                            ErrorTextBlock.Text = "Please select a database from databases.";
                            CoBoxDB.Foreground = Brushes.Orange;
                            ImageErrorGif.Visibility = Visibility.Visible;
                            ErrorDelay();
                            transferedTextBlock.Text = "";
                            successTextBlock.Text = "";
                            ImageRightGif.Visibility = Visibility.Hidden;
                            infoTextBlock.Text = "";
                            ImageInfoGif.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            MySqlCommand cmd = new($"drop database {CoBoxDB.Text}", conn);
                            cmd.ExecuteNonQuery();
                            //ProgressBar();
                            ImageRightGif.Visibility = Visibility.Visible;
                            successTextBlock.Text = "Database has been successfully deleted.";
                            SuccessDelay();
                            transferedTextBlock.Text = "";
                            ErrorTextBlock.Text = "";
                            ImageErrorGif.Visibility = Visibility.Hidden;
                            infoTextBlock.Text = "";
                            ImageInfoGif.Visibility = Visibility.Hidden;
                        }

                    }
                    else
                    {
                        ImageErrorGif.Visibility = Visibility.Visible;
                        ErrorTextBlock.Text = "Could not delete the database.";
                        ErrorDelay();
                        transferedTextBlock.Text = "";
                        successTextBlock.Text = "";
                        ImageRightGif.Visibility = Visibility.Hidden;
                        infoTextBlock.Text = "";
                        ImageInfoGif.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    ImageErrorGif.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = ex.Message;
                    ErrorDelay();
                    transferedTextBlock.Text = "";
                    successTextBlock.Text = "";
                    ImageRightGif.Visibility = Visibility.Hidden;
                    infoTextBlock.Text = "";
                    ImageInfoGif.Visibility = Visibility.Hidden;
                }
            }
            else if (CoBoxSourceDBType.Text == "LOCAL")
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "The Delete button only deletes databases from MySQL and SQL but not from LOCAL.";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
            else if (CoBoxSourceDBType.Text == "SQL")
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "The Delete button has not yet been programmed for SQL databases.";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
            else
            {
                ImageErrorGif.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Please select a database type!";
                ErrorDelay();
                transferedTextBlock.Text = "";
                successTextBlock.Text = "";
                ImageRightGif.Visibility = Visibility.Hidden;
                infoTextBlock.Text = "";
                ImageInfoGif.Visibility = Visibility.Hidden;
            }
        }

        ////////// Button mouse over (MouseEnter, MouseLeave) ///////////

        MediaPlayer player = new MediaPlayer();

        // Attach button mouse over
        private void BtnAttach_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxDBName.BorderBrush = Brushes.Red;
            TxBxDBPath.BorderBrush = Brushes.Red;
            TxBxTargetServer.BorderBrush = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = $"1-Select database type as LOCAL.\n2-Enter a source server.\n3-Enter a database file path.\n4-Enter a database name.\n";
            infoTextBlock.Inlines.Add(new Bold(new Italic(new Run("Attach is just for local server!"))));
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;

            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnAttach_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxDBName.BorderBrush = Brushes.White;
            TxBxDBPath.BorderBrush = Brushes.White;
            TxBxTargetServer.BorderBrush = Brushes.White;
            ImageInfoGif.Visibility = Visibility.Hidden;
            infoTextBlock.Text = "";
        }

        // Detach button mouse over
        private void BtnDetach_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.Red;
            BtnGetDB.Background = Brushes.Red;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select database type as LOCAL.\n2-Enter source server.\n3-Click button 'Get Databases from Server'.\n4-Select a database.\n";
            infoTextBlock.Inlines.Add(new Bold(new Italic(new Run("Detach is just for local server!"))));
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnDetach_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.White;
            BtnGetDB.Background = Brushes.Transparent;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        // Transfer button mouse over
        private void BtnTransfer_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.Red;
            TxBxSourceUserName.BorderBrush = Brushes.Red;
            TxBxSourcePassword.BorderBrush = Brushes.Red;
            TxBxSourcePort.BorderBrush = Brushes.Red;
            BtnGetDB.Background = Brushes.Red;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.Red;

            TxBxTargetServer.BorderBrush = Brushes.Red;
            TxBxTargetUserName.BorderBrush = Brushes.Red;
            TxBxTargetPassword.BorderBrush = Brushes.Red;
            TxBxTargetPort.BorderBrush = Brushes.Red;
            TxBxDBPath.BorderBrush = Brushes.Red;
            TxBxDBName.BorderBrush = Brushes.Red;

            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select database type in source and target server.\n2-Enter server, username, password and port in source and Target Server.\n" +
                "3-Click button 'Get Databases from Server' and select a database.\n4-Enter the path to temporarily save the data during the transfer process.\n" +
                "5-Enter the file name and it must be the same name as in the source.";
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnTransfer_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.White;
            TxBxSourceUserName.BorderBrush = Brushes.White;
            TxBxSourcePassword.BorderBrush = Brushes.White;
            TxBxSourcePort.BorderBrush = Brushes.White;
            BtnGetDB.Background = Brushes.Transparent;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.White;

            TxBxTargetServer.BorderBrush = Brushes.White;
            TxBxTargetUserName.BorderBrush = Brushes.White;
            TxBxTargetPassword.BorderBrush = Brushes.White;
            TxBxTargetPort.BorderBrush = Brushes.White;
            TxBxDBPath.BorderBrush = Brushes.White;
            TxBxDBName.BorderBrush = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        // Delete button mouse over
        private void BtnDelete_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.Red;
            TxBxSourceUserName.BorderBrush = Brushes.Red;
            TxBxSourcePassword.BorderBrush = Brushes.Red;
            TxBxSourcePort.BorderBrush = Brushes.Red;
            BtnGetDB.Background = Brushes.Red;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select database type as MySQL or SQL.\n2-Enter server, username and password in Source Server.\n3-Click button 'Get Databases from Server'.\n4-Select a database.\n";
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnDelete_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.White;
            TxBxSourceUserName.BorderBrush = Brushes.White;
            TxBxSourcePassword.BorderBrush = Brushes.White;
            TxBxSourcePort.BorderBrush = Brushes.White;
            BtnGetDB.Background = Brushes.Transparent;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        // Export button mouse over
        private void BtnExport_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.Red;
            TxBxSourceUserName.BorderBrush = Brushes.Red;
            TxBxSourcePassword.BorderBrush = Brushes.Red;
            TxBxSourcePort.BorderBrush = Brushes.Red;
            TxBxDBPath.BorderBrush = Brushes.Red;
            BtnGetDB.Background = Brushes.Red;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select database type.\n2-Enter server, username, password and port in Source Server.\n3-Click button 'Get Databases from Server'.\n4-Select a database.\n5-Enter the path you want to export the file to.\n";
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnExport_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.White;
            TxBxSourceUserName.BorderBrush = Brushes.White;
            TxBxSourcePassword.BorderBrush = Brushes.White;
            TxBxSourcePort.BorderBrush = Brushes.White;
            TxBxDBPath.BorderBrush = Brushes.White;
            BtnGetDB.Background = Brushes.Transparent;
            BtnGetDB.Foreground = Brushes.White;
            CoBoxDB.Foreground = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        // Import button mouse over
        private void BtnImport_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxTargetServer.BorderBrush = Brushes.Red;
            TxBxTargetUserName.BorderBrush = Brushes.Red;
            TxBxTargetPassword.BorderBrush = Brushes.Red;
            TxBxTargetPort.BorderBrush = Brushes.Red;
            TxBxDBPath.BorderBrush = Brushes.Red;
            TxBxDBName.BorderBrush = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select database type.\n2-Enter server, username, password and port in Target Server.\n3-Enter the path you want to import the file from.\n4-Enter the database name.";
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnImport_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxTargetServer.BorderBrush = Brushes.White;
            TxBxTargetUserName.BorderBrush = Brushes.White;
            TxBxTargetPassword.BorderBrush = Brushes.White;
            TxBxTargetPort.BorderBrush = Brushes.White;
            TxBxDBPath.BorderBrush = Brushes.White;
            TxBxDBName.BorderBrush = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        // Get DB button mouse over
        private void BtnGetDB_MouseEnter(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.Red;
            TxBxSourceUserName.BorderBrush = Brushes.Red;
            TxBxSourcePassword.BorderBrush = Brushes.Red;
            TxBxSourcePort.BorderBrush = Brushes.Red;
            ImageInfoGif.Visibility = Visibility.Visible;
            infoTextBlock.Text = "1-Select the database type.\n2-Enter server, username and password in Source Server.\n3-Select a database.";
            ErrorTextBlock.Text = "";
            successTextBlock.Text = "";
            transferedTextBlock.Text = "";
            ImageErrorGif.Visibility = Visibility.Hidden;
            ImageRightGif.Visibility = Visibility.Hidden;
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }
        private void BtnGetDB_MouseLeave(object sender, MouseEventArgs e)
        {
            TxBxSourceServer.BorderBrush = Brushes.White;
            TxBxSourceUserName.BorderBrush = Brushes.White;
            TxBxSourcePassword.BorderBrush = Brushes.White;
            TxBxSourcePort.BorderBrush = Brushes.White;
            infoTextBlock.Text = "";
            ImageInfoGif.Visibility = Visibility.Hidden;
        }

        private void BtnShowDb_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnShowDb_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void ChangePassword_MouseEnter(object sender, MouseEventArgs e)
        {
            player.Open(new Uri(@"D:\OneDrive - bfw-schoemberg.de\Team IT-S-21-01\Anwendungsentwicklung\CSharp\Ein Fenster mit Tabs Projekt\MyApp\MyApp\sound\button.wav"));
            player.Play();
        }

        private void BtnShowDb_Click(object sender, RoutedEventArgs e)
        {
            ShowDatabase();

        }

        public void ShowDatabase()
        {
            int newIndex = Tab_Control.SelectedIndex + 1;
            if (newIndex >= Tab_Control.Items.Count)
                newIndex = 0;
            Tab_Control.SelectedIndex = newIndex;

            DbLabel.Content = CoBoxDB.SelectedItem.ToString();


            string server = TxBxSourceServer.Text;
            string userName = TxBxSourceUserName.Text;
            string pass = TxBxSourcePassword.Password;
            string port = TxBxSourcePort.Text;
            string db = CoBoxDB.Text;
            string connectionString = "host='" + server + "'; user='" + userName + "'; Password='" + pass + "'; database= '" + db + "'; port= '" + port + "'; Connection Timeout=30;";

            MySqlConnection conn = new MySqlConnection(connectionString);

            MySqlCommand comm = new MySqlCommand();

            comm.Connection = conn;
            conn.Open();

            comm.CommandText = $"Use {db}";
            comm.ExecuteNonQuery();

            comm.CommandText = "show Tables";
            comm.ExecuteNonQuery();

            ListBoxTableName.Items.Clear();
            ListBoxTableName.Items.Add("New Table +");
            MySqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                //LBItemTableList.Content += reader[$"Tables_in_{db}"].ToString();
                ListBoxTableName.Items.Add(reader[$"Tables_in_{db}"]).ToString();
            }
        }



        //////////////////////   Database Tab   ///////////////////////
        private void CreateNewTable_Click(object sender, RoutedEventArgs e)
        {
            NewTableStackPanel.Visibility = Visibility.Visible;
        }

        private void BtnCreateNewTable_Click(object sender, RoutedEventArgs e)
        {
            int rowsToAdd = Convert.ToInt32(TxBxColumnsCount.Text);

            for (int x = 0; x < rowsToAdd; x++)
            {
                NewRows.Add(new AddRowClass());
            }
            int newIndex = Tab_Control.SelectedIndex + 1;
            if (newIndex >= Tab_Control.Items.Count)
                newIndex = 0;
            Tab_Control.SelectedIndex = newIndex;
            TableNameLabel.Content = TxBxTableName.Text;
            ColumnsCountLable.Content = TxBxColumnsCount.Text;
            

        }


        private void LBITable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void ListBoxTableName_MouseDown(object sender, MouseButtonEventArgs e)
        {

            int tablesElement = 0;
            string server = TxBxSourceServer.Text;
            string userName = TxBxSourceUserName.Text;
            string pass = TxBxSourcePassword.Password;
            string port = TxBxSourcePort.Text;
            string db = CoBoxDB.Text;
            string connectionString = "host='" + server + "'; user='" + userName + "'; Password='" + pass + "'; database= '" + db + "'; port= '" + port + "'; Connection Timeout=30;";
            MySqlConnection con = new MySqlConnection(connectionString);
            MySqlCommand com = new MySqlCommand($"Select * from {ListBoxTableName.Items}", con);
            
            for (int i = 0; i < tablesElement; i++)
            {
                ColumnName.Header = com;
                com.ExecuteNonQuery();
            }
            int newIndex = Tab_Control.SelectedIndex + 3;
            if (newIndex >= Tab_Control.Items.Count)
                newIndex = 0;
            Tab_Control.SelectedIndex = newIndex;
            TableContentTab.Header = ListBoxTableName.Items;
        }

        /////////////////////   New Table Tab  ////////////////////////

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                AddRowClass addRow = e.Row.Item as AddRowClass;
                CreateTable(addRow);
            }
        }


        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            NewRows.Add(new AddRowClass());
            ColumnsCountLable.Content = Convert.ToInt32(ColumnsCountLable.Content) + 1;
        }

        private void BtnRemoveRow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCreateTable_Click(object sender, RoutedEventArgs e)
        {
            //CreateTable();

        }

        private void CreateTable(AddRowClass addRow)
        {
            DataTable dt = new DataTable();
            List<AddRowClass> listAddRow = new List<AddRowClass>();
            string server = TxBxSourceServer.Text;
            string user = TxBxSourceUserName.Text;
            string pass = TxBxSourcePassword.Password;
            string port = TxBxSourcePort.Text;
            string db = CoBoxDB.Text;
            string connectionString = "host='" + server + "'; database='" + db + "'; user='" + user + "'; Password='" + pass + "'; port= '" + port + "'; Connection Timeout=30;";

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                conn.Open();
                comm.CommandText = "USE " + db;
                comm.ExecuteNonQuery();

                //comm.CommandText = $"CREATE TABLE {TableNameLabel.Content} ({DaGrTexColName.Header} {DaGrTexColType.Header}({DaGrTexColLength.Header}) {DaGrTexColNullValue.Header})";
                //comm.CommandText = "CREATE TABLE TestTable (id int(14) not null, firstName nvarchar(25), lastName nvarchar(25))";
                comm.CommandText = $"CREATE TABLE {TableNameLabel.Content} (@ColumnName @Type(@Length) @NullValue)";
                comm.Parameters.AddWithValue("@ColumnName", addRow.ColumnName);
                comm.Parameters.AddWithValue("@Type", addRow.Type);
                comm.Parameters.AddWithValue("@Length", addRow.Length);
                comm.ExecuteNonQuery();
                dt.Load(comm.ExecuteReader());
                foreach (DataRow row in dt.Rows)
                {
                    AddRowClass adrc = new AddRowClass(/*row[0].ToString(), row[1].ToString(), Convert.ToInt32(row[2]), Convert.ToBoolean(row[3])*/);
                    listAddRow.Add(adrc);
                }

                successTextBlock.Text = "Table has been successfully created";
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                ErrorTextBlock.Text = ex.Message;
            }
            conn.Close();
        }
            
        public class AddRowClass
        {
            public string? ColumnName { get; set; }
            public string? Type { get; set; }
            public int Length { get; set; }
            public bool NullValue { get; set; }
            //public AddRowClass(string col1, string col2, int col3, bool col4)
            //{
            //    Name = col1;
            //    Type = col2;
            //    Length = col3;
            //    NullValue = col4;
            //}
        }


        private void SelectPathIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TxBxDBPath.Text = Path.Combine(folder.SelectedPath, Path.GetFileName(CoBoxDB.Text));

                TxBxDBName.Text = CoBoxDB.Text;
            }
        }

    }
}
