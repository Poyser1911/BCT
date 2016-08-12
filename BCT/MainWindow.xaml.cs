using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Threading;
using Q3query;
using Shell32;
using IWshRuntimeLibrary;

namespace BCT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Globals 
        static string ip = "38.130.203.243";
        static int port = 28930;
        static query d = new query(ip, port);
        Dictionary<string, string> dvars = d.GetInfoDvars();
        Thread refresh;
        #endregion 

        #region Main
        public MainWindow()
        {
            InitializeComponent();

           refresh = new Thread(AutoRefresh);
           refresh.Start();
        }
        #endregion

        #region Auto Refresh
        public void AutoRefresh()
        {
            bool autore = false;
            while (true)
            {
                Dispatcher.Invoke(new Action(() =>
{
    autore = (bool)autorefresh.IsChecked;
}));
                while (autore)
                {
                    DataTable players = d.GetplayersTable();
                    Dispatcher.Invoke(new Action(() =>
    {
    if (online == null)
        return;
    Dictionary<string, string> dvars = d.GetInfoDvars();
    playersbox.Items.Clear();
    playersbox2.Items.Clear();
    playersbox3.Items.Clear();
    if (players == null || dvars == null)
    {
        online.Content = "Sever Offline - Check Internet";
        return;
    }
    online.Content = "Players Online: " + players.Rows.Count + "/" + dvars["sv_maxclients"] + "  Gametype: " + dvars["g_gametype"] + "  Map: " + dvars["mapname"].Substring(2).Substring(0, 2).ToUpper() + dvars["mapname"].Substring(4);
    foreach (DataRow dr in players.Rows)
    {
        RichTextBox x = new RichTextBox();
        x.Background = Brushes.Transparent;
        x.BorderBrush = Brushes.Transparent;
        x.BorderThickness = new Thickness(0, 0, 0, 0);
        x.Width = playersbox.Width;
        x.IsReadOnly = true;
        d.Q3Out(x, dr["name"].ToString());
        playersbox.Items.Add(x);
        playersbox2.Items.Add(dr["score"]);
        playersbox3.Items.Add(dr["ping"]);
    }
    autore = (bool)autorefresh.IsChecked;
}));
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }

        }
        #endregion

        #region Kill Processs 
        private void Window_Closed_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            p.Kill();
        }
        #endregion

        #region Join Server
        public void joinserver(string ip, string port)
        {
            if (Properties.Settings.Default.gamepath == string.Empty)
            {
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Do you want to set game path?", "Game path not found", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    SetupPath();
                return;
            }

            try
            {
                string inkpath = AppDomain.CurrentDomain.BaseDirectory + "\\cod4game.lnk";
                var wsh = new IWshShell_Class();
                IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(inkpath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Arguments = "connect " + ip + ":" + port;
                shortcut.TargetPath = Properties.Settings.Default.gamepath;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(Properties.Settings.Default.gamepath);
                shortcut.Save();

                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\cod4game.lnk");
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\cod4game.lnk");
            }
            catch (Exception c)
            {
                if (c.Message.Contains("Access is denied"))
                    MessageBox.Show("Try moving files to Desktop/Documents", "Unable to Launch Game", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(c.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetupPath()
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.Filter = "Cod4|iw3mp.exe|Renamed it(*.exe)|*.exe|All Files(*.*)|*.*";
            if (System.IO.Directory.Exists(@"C:\Program Files (x86)\Activision\Call of Duty 4 - Modern Warfare"))
                file.InitialDirectory = @"C:\Program Files (x86)\Activision\Call of Duty 4 - Modern Warfare";
            else if (System.IO.Directory.Exists(@"C:\Program Files (x86)\Steam\SteamApps\common\Call of Duty 4"))
                file.InitialDirectory = @"C:\Program Files (x86)\Steam\SteamApps\common\Call of Duty 4";

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Set Game Path: " + file.FileName, "Save Settings", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.gamepath = file.FileName;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Saved: " + Properties.Settings.Default.gamepath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }

        private void join_Click(object sender, RoutedEventArgs e)
        {
            joinserver(ip, port.ToString());
        }

        private void logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            joinserver(ip, port.ToString());
        }
        #endregion

    }
}
