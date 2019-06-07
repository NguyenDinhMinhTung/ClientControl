using ClientControl.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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

namespace ClientControl
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String serverIP = "14.9.118.64";
        private const int serverPort = 8530;

        private const int localPort = 7574;

        UDPProtocol udpProtocol;

        ObservableCollection<IPPort> listIPPort;

        IPEndPoint clientIP;

        public MainWindow()
        {
            InitializeComponent();

            listIPPort = new ObservableCollection<IPPort>();
            listViewIPPort.ItemsSource = listIPPort;

            udpProtocol = new UDPProtocol(localPort);
            udpProtocol.UdpSocketReceiveStart(RunCommand);
            SendIPPort();
            RefreshIPPortList();
        }

        public void RunCommand(IPEndPoint ipEndPoint, byte[] command)
        {
            switch (command[0])
            {
                case 1:
                    break;

                case 4:
                    String strList = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    String[] split = strList.Split('|');

                    Dispatcher.Invoke(() =>
                    {
                        listIPPort.Clear();
                    });


                    int i = 0;
                    while (i < split.Length)
                    {
                        int id = int.Parse(split[i++]);
                        int userid = int.Parse(split[i++]);
                        String ip = split[i++];
                        int port = int.Parse(split[i++]);
                        DateTime dateTime = DateTime.Parse(split[i++]);

                        IPPort ipPort = new IPPort(id, userid, ip, port, dateTime);
                        Dispatcher.Invoke(() =>
                        {
                            listIPPort.Add(ipPort);
                        });
                    }

                    break;

                case 7:

                    break;
            }
        }

        public void SendIPPort()
        {
            udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 3, 1 });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (udpProtocol != null)
            {
                udpProtocol.Close();
            }
        }

        public void RefreshIPPortList()
        {
            udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 4 });
        }

        private void BtnRefreshIPPortList_Click(object sender, RoutedEventArgs e)
        {
            RefreshIPPortList();
        }

        private void BtnChat_Click(object sender, RoutedEventArgs e)
        {
            if (listViewIPPort.SelectedIndex >= 0)
            {
                IPPort ipport = listViewIPPort.SelectedItems[0] as IPPort;
                int userid = ipport.UserId;

                clientIP = new IPEndPoint(IPAddress.Parse(ipport.IP), ipport.Port);

                udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 7, (byte)userid });

                sendChatMess("HELLO");
            }
        }

        private void sendChatMess(String text)
        {
            if (clientIP == null) return;

            byte[] textbyte = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] sendbyte = new byte[textbyte.Length + 1];

            sendbyte[0] = 8;

            Array.Copy(textbyte, 0, sendbyte, 1, textbyte.Length);

            udpProtocol.UdpSocketSend(clientIP, sendbyte);
        }
    }
}
