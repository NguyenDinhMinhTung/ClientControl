using ClientControl.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private const int localPort = 46800;

        UDPProtocol udpProtocol;

        ObservableCollection<IPPort> listIPPort;
        ObservableCollection<User> listUser;

        ChatWindowManager chatWindowManager;
        ViewScreenManager viewScreenManager;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                //System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("14.9.118.64", 8530);
                //udpClient.Send(new byte[] { 0 }, 1);

                udpProtocol = new UDPProtocol(localPort);
                udpProtocol.UdpSocketReceiveStart(RunCommand);

                listIPPort = new ObservableCollection<IPPort>();
                listUser = new ObservableCollection<User>();

                chatWindowManager = new ChatWindowManager(udpProtocol);
                viewScreenManager = new ViewScreenManager(udpProtocol);

                listViewUsers.ItemsSource = listUser;

                SendIPPort();
                RefreshUserList();
                GetIPPortList();
                EnableControlButton(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public void RunCommand(IPEndPoint ipEndPoint, byte[] command)
        {
            switch (command[0])
            {
                case 1:
                    break;

                case 4:
                    String strListIPPort = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    listIPPort = IPPort.GetListIPPort(strListIPPort);
                    break;

                case 7:

                    break;

                case 8:
                    IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == command[1]; }).FirstOrDefault();
                    String mess = System.Text.Encoding.UTF8.GetString(command, 3, command.Length - 3);

                    chatWindowManager.ShowChatWindow(ipport, mess);
                    break;

                case 9:
                    String strListUser = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    listUser = User.GetListUser(strListUser);
                    listViewUsers.Dispatcher.Invoke(() =>
                    {
                        listViewUsers.ItemsSource = listUser;
                    });
                    break;

                case 10:
                    int x = BitConverter.ToInt32(command.Skip(3).Take(4).ToArray(), 0);
                    int y = BitConverter.ToInt32(command.Skip(7).Take(4).ToArray(), 0);

                    BitmapSource screenImage = BitmapSourceFromArray(command.Skip(11).ToArray());
                    screenImage.Freeze();

                    viewScreenManager.SetImage(command[1], screenImage, x, y);
                    break;
            }
        }

        private BitmapSource BitmapSourceFromArray(byte[] data)
        {
            JpegBitmapDecoder jpegBitmapDecoder = new JpegBitmapDecoder(new MemoryStream(data), BitmapCreateOptions.None, BitmapCacheOption.Default);
            return jpegBitmapDecoder.Frames[0];
        }

        public void SendIPPort()
        {
            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, new byte[] { 3, 1 });
        }

        private void GetIPPortList()
        {
            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, new byte[] { 4 });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (udpProtocol != null)
            {
                udpProtocol.Close();
            }
        }

        public void RefreshUserList()
        {
            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, new byte[] { 9 });
        }

        private void BtnRefreshUserList_Click(object sender, RoutedEventArgs e)
        {
            RefreshUserList();
            GetIPPortList();
        }

        private void BtnChat_Click(object sender, RoutedEventArgs e)
        {
            if (listViewUsers.SelectedIndex >= 0)
            {
                User user = listViewUsers.SelectedItem as User;
                IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == user.ID; }).FirstOrDefault();

                if (!udpProtocol.isServerOver) EstablishConnection(user, ipport);
                chatWindowManager.ShowChatWindow(ipport);
            }
        }

        private void ShowUserDetaiInfo(User user)
        {
            if (user != null)
            {
                IPPort ipPort = listIPPort.Where(ipport => { return ipport.UserId == user.ID; }).FirstOrDefault();
                txtUserName.Text = user.Name;
                txtIP.Text = "IP: " + ipPort.IP;
                txtPort.Text = "Port: " + ipPort.Port;
            }
            else
            {
                txtUserName.Text = "";
                txtIP.Text = "IP: ";
                txtPort.Text = "Port: ";
            }
        }

        private void EnableControlButton(Boolean isEnable)
        {
            btnChat.IsEnabled = btnFileExplorer.IsEnabled =
            btnMessageBox.IsEnabled = btnRestart.IsEnabled =
            btnShutdown.IsEnabled = btnViewScreen.IsEnabled =
            btnChangeName.IsEnabled = ckbOverServer.IsEnabled = isEnable;
        }

        private void EstablishConnection(User user, IPPort iPPort)
        {
            //IPPort iPPort = listIPPort.Where((ipp) => { return ipp.UserId == user.ID; }).FirstOrDefault();

            byte[] data = new byte[] { 7, 1, (byte)user.ID };
            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, data);

            System.Threading.Thread.Sleep(50);
            for (int i = 0; i < 5; i++)
            {
                udpProtocol.UdpSocketSend(iPPort.IP, iPPort.Port, new byte[] { 0, 1, (byte)user.ID });
            }
        }

        private void ListViewUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewUsers.SelectedIndex < 0)
            {
                EnableControlButton(false);
                ShowUserDetaiInfo(null);
            }
            else
            {
                EnableControlButton(true);
                ShowUserDetaiInfo(listViewUsers.SelectedItem as User);
            }
        }

        private void BtnViewScreen_Click(object sender, RoutedEventArgs e)
        {
            User user = listViewUsers.SelectedItem as User;

            IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == user.ID; }).FirstOrDefault();
            //int userid = ipport.UserId;

            //udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 10, (byte)userid });

            //udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 0 });
            //udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 0 });

            if (!udpProtocol.isServerOver) EstablishConnection(user, ipport);

            viewScreenManager.ShowViewScreenWindow(ipport);
        }

        private void BtnChangeName_Click(object sender, RoutedEventArgs e)
        {
            User user = listViewUsers.SelectedItem as User;
            ChangeNameWindow changeNameWindow = new ChangeNameWindow(user.Name, str =>
            {
                byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(str);
                byte[] sendData = new byte[] { 2, (byte)user.ID }.Concat(nameBytes).ToArray();
                udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, sendData);
                RefreshUserList();
                GetIPPortList();
            });

            changeNameWindow.ShowDialog();
        }

        private void CkbOverServer_Checked(object sender, RoutedEventArgs e)
        {
            udpProtocol.isServerOver = true;
            User user = listViewUsers.SelectedItem as User;
            IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == user.ID; }).FirstOrDefault();

            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, new byte[] { 12, 1, (byte)user.ID, 1 });
        }

        private void CkbOverServer_Unchecked(object sender, RoutedEventArgs e)
        {
            udpProtocol.isServerOver = false;
            User user = listViewUsers.SelectedItem as User;
            IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == user.ID; }).FirstOrDefault();

            udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, new byte[] { 12, 1, (byte)user.ID, 0 });
        }
    }
}
