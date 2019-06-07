using System;
using System.Collections.Generic;
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


        public MainWindow()
        {
            InitializeComponent();

            udpProtocol = new UDPProtocol(localPort);
            udpProtocol.UdpSocketReceiveStart(RunCommand);
            SendIPPort();
        }

        public void RunCommand(IPEndPoint ipEndPoint, byte[] data)
        {

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
    }
}
