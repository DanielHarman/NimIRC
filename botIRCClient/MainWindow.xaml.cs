using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace botIRCClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private static ircobj client = new ircobj();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "IRCClient - Not Connected";
            client.logText.CollectionChanged += update;
        }

        private void btnConn_Click(object sender, RoutedEventArgs e)
        {
            client.setServerIP(txtIP.Text);
            client.setNick(txtName.Text);
            client.connect();
            txtClientName.Text = client.getNick();
        }

        private void btnSay_Click(object sender, RoutedEventArgs e)
        {
            client.say(txtSay.Text, "#nimphina");
        }

        private void txtSay_KeyDown(object sender, KeyEventArgs e)
        {
            //Enter bit for scrolling through previous output
            if (e.Key == Key.Enter)
            {
                client.say(txtSay.Text, "#nimphina");
                txtSay.Text = "";
            }
        }

        private void update(object sender, NotifyCollectionChangedEventArgs args)
        {

            string newtxt = "";
            foreach (string item in client.logText)
            {
                newtxt += item + " \n";
            }
            this.Dispatcher.Invoke((Action)(() => { txtLogBox.Text = newtxt; }));
        }
    }
}
