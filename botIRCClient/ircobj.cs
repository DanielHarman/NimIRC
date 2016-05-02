using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Meebey.SmartIrc4net;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace botIRCClient
{
    class ircobj
    {

        private IrcClient irc = new IrcClient();
        private List<string> channels = new List<string>();
        public ObservableCollection<string> logText = new ObservableCollection<string>();

        private DateTime startTime = DateTime.Now;
        private string version = "MK3";
        private string nick;
        private string serverIP;
        private int port = 6667;
        private bool connected = false;
        private Thread listening;// = new Thread(irc.Listen);


        public ircobj()
        {
            //Set irc library related options
            irc.Encoding = System.Text.Encoding.UTF8;
            irc.ActiveChannelSyncing = true;
            irc.AutoReconnect = true;
            irc.AutoRetry = true;
            irc.AutoRetryDelay = 10;
            irc.AutoRelogin = true;
            irc.AutoJoinOnInvite = true;
            irc.CtcpVersion = nick + " " + version;
            irc.SendDelay = 300;

            irc.OnPing += new PingEventHandler(onPing);
            irc.OnConnected += new EventHandler(onConnected);
            irc.OnDisconnected += new EventHandler(onDisconnected);
            irc.OnRawMessage += new IrcEventHandler(onRawMessage);
        }


        public void writeLogList(string s)
        {
            if (s != "")
            {
                this.logText.Add(s);
            }

            if (this.logText.Count > 10)
            {
                this.logText.RemoveAt(0);
            }

        }

        public void setNick(string botname)
        {
            if (botname != "")
            {
                this.nick = botname;
                if (connected)
                {
                    irc.RfcNick(nick);
                }
                writeLogList(String.Format("Set name to: {0}", botname));
            }
            else
            {
                writeLogList("Botname cannot be empty");
            }
        }

        public void setServerIP(string ip)
        {
            if (ip != "" && !connected)
            {
                this.serverIP = ip;
            }
            else if (connected)
            {
                writeLogList("Cannot change server ip while connected");
            }
            else
            {
                writeLogList("IP cannot be empty");
            }
        }

        public void setPort(int port)
        {

        }

        public void connect()
        {
            if (this.nick != "" && this.serverIP != "" && !this.connected)
            {
                writeLogList(String.Format("Attempting to connect to {0} on port {1}", this.serverIP, this.port));
                irc.Connect(this.serverIP, this.port);
                connected = true;
            }
            else if (this.connected)
            {
                writeLogList("Already connected to a server");
            }


        }

        public void disconnect()
        {
            if (this.connected)
            {
                irc.Disconnect();
                connected = false;
                listening.Abort();
            }
            else
            {

            }
        }

        public DateTime getUptime()
        {
            return startTime;
        }

        public string getNick()
        {
            return nick;
        }

        public void joinChan(string channel)
        {
            if (channel != "" && !channels.Contains(channel))
            {
                irc.RfcJoin(channel);
                channels.Add(channel);
            }
        }

        public void partChan(string channel)
        {

        }

        public void say(string s, string channel)
        {

            if (s.StartsWith("/"))
            {
                s = s.Substring(1);
                string[] command = s.TrimEnd().Split(' '); // Regex.Split(s, @"\W+");

                if (!String.IsNullOrEmpty(command[1]))
                {
                    switch (command[0])
                    {
                        case ("join"):
                            joinChan(command[1]);
                            break;

                        case ("part"):
                        case ("leave"):
                            partChan(command[1]);
                            break;

                        case ("nick"):
                            setNick(command[1]);
                            break;
                    }
                }
            }
            else
            {
                irc.SendMessage(SendType.Message, channel, s);
                writeLogList(String.Format("[{0}] <{1}>: {2}", channel, nick, s));
            }

        }

        private void onConnected(object sender, EventArgs e)
        {
            irc.Login(nick, nick, 0, nick);
            writeLogList("Connected!");
            listening = new Thread(irc.Listen);
            listening.Start();
        }

        private void onDisconnected(object sender, EventArgs e)
        {

        }

        private void onRawMessage(object sender, IrcEventArgs e)
        {
            writeLogList(String.Format("[{3}][{0}] <{1}>:{2}", e.Data.Channel, e.Data.Nick, e.Data.Message, DateTime.Now.ToShortTimeString()));

            if (!String.IsNullOrEmpty(e.Data.Message))
            {
                if (e.Data.Message == "#hello")
                {
                    say(String.Format("Hello {0}", e.Data.Nick), e.Data.Channel);
                }
            }

        }

        private void onPing(object sender, PingEventArgs e)
        {


        }
    }
}
