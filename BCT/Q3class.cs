using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;


/* q3query.dll - Quake 3 query class
 *
 * Copyright (C) 2015 Damani Poyser
 * Author(s): Damani Poyser (damanipoyser1911@yahoo.com)
 *
 */

namespace Q3query
{
    public class query
    {
        string rconpass = null;

        Socket sck;
        IPEndPoint server;

        public query(string ip, int port)
        {
            //IPHostEntry Host = Dns.GetHostEntry(ip);
            //MessageBox.Show(Host.AddressList[0].ToString());
            server = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void SetRconPassword(string pass)
        {
            rconpass = pass;
        }

        public byte[] Tobyte(string msg)
        {
            byte[] bufferTemp = Encoding.ASCII.GetBytes(msg);
            byte[] bufferSend = new byte[bufferTemp.Length + 4];


            for (int i = 0; i < 4; i++)
                bufferSend[i] = 0xFF;
            for (int i = 0; i < bufferTemp.Length; i++)
                bufferSend[i + 4] = bufferTemp[i];

            return bufferSend;

        }
        public string Send(string msg)
        {
            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sck.Connect(server);

                byte[] buffer = new byte[2000];
                sck.Send(Tobyte(msg));

                sck.ReceiveTimeout = 600;
                sck.Receive(buffer);
                return Encoding.Default.GetString(buffer).Replace('\0', ' ').Trim();
            }
            catch (Exception)
            {

                return null;
            }

        }

        public Dictionary<string, string> GetInfoDvars()
        {
            string status = Send("getstatus");
            if (status == null)
                return null;
            string[] dvarlist = status.Split('\n')[1].Split('\\');
            var dvars = new Dictionary<string, string>();

            for (int i = 1; i < dvarlist.Length; i += 2)
                dvars[dvarlist[i]] = dvarlist[i + 1];

            return dvars;
        }

        public DataTable GetplayersTable()
        {
            string status = Send("getstatus");
            if (status == null)
                return null;

            string[] playerlist = status.Split('\n');
            DataTable playerstable = new DataTable();
            playerstable.Columns.Add(new DataColumn("name"));
            playerstable.Columns.Add(new DataColumn("score"));
            playerstable.Columns.Add(new DataColumn("ping"));

            for (int i = 2; i < playerlist.Length; i++)
                playerstable.Rows.Add(playerlist[i].Split('"')[1], playerlist[i].Split(' ')[0], playerlist[i].Split(' ')[1]);
            return playerstable;
        }

        public string Rconcmd(string cmd)
        {
            if (rconpass == null)
                return "Rconpassword Not Set use function SetRconPassword(yourpass) first";
            return Send("rcon " + rconpass + " " + cmd).Substring(9).Trim();
        }

        public void Q3Out(System.Windows.Controls.RichTextBox help, string text)
        {
            var colour = Brushes.White;
            help.Document.Blocks.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '^')
                {
                    switch (text[i + 1])
                    {
                        case '1': colour = Brushes.Red;
                            break;
                        case '2': colour = Brushes.Green;
                            break;
                        case '3': colour = Brushes.Yellow;
                            break;
                        case '4': colour = Brushes.Blue;
                            break;
                        case '5': colour = Brushes.Aqua;
                            break;
                        case '6': colour = Brushes.Purple;
                            break;
                        case '7': colour = Brushes.White;
                            break;
                        case '8': colour = Brushes.Gray;
                            break;
                        case '9': colour = Brushes.Brown;
                            break;
                        case '0': colour = Brushes.Black;
                            break;
                        default:
                            break;
                    }
                    i = i + 2;
                    TextRange rangeOfWord = new TextRange(help.Document.ContentEnd, help.Document.ContentEnd);
                    rangeOfWord.Text = text[i].ToString();
                    rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, colour);
                    rangeOfWord.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
                }
                else
                {
                    TextRange rangeOfWord = new TextRange(help.Document.ContentEnd, help.Document.ContentEnd);
                    rangeOfWord.Text = text[i].ToString();
                    rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, colour);
                    rangeOfWord.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
                }
            }
        }
    }
}
