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
        private const String serverIP = "14.9.118.64";
        private const int serverPort = 8530;

        private const int localPort = 46800;

        UDPProtocol udpProtocol;

        ObservableCollection<IPPort> listIPPort;
        ObservableCollection<User> listUser;

        List<ChatWindow> listChatWindow;
        List<ViewScreenWindow> ListViewScreenWindow;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                //System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("14.9.118.64", 8530);
                //udpClient.Send(new byte[] { 0 }, 1);

                listIPPort = new ObservableCollection<IPPort>();
                listUser = new ObservableCollection<User>();
                listChatWindow = new List<ChatWindow>();
                ListViewScreenWindow = new List<ViewScreenWindow>();

                listViewUsers.ItemsSource = listUser;

                udpProtocol = new UDPProtocol(localPort);
                udpProtocol.UdpSocketReceiveStart(RunCommand);

                SendIPPort();
                RefreshUserList();
                GetIPPortList();
                enableControlButton(false);
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

                case 8:
                    IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == command[1]; }).FirstOrDefault();
                    String mess = System.Text.Encoding.UTF8.GetString(command, 2, command.Length - 2);

                    ShowChatWindow(ipport, mess);
                    break;

                case 9:
                    String strListUser = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    String[] splitListUser = strListUser.Split('|');

                    Dispatcher.Invoke(() =>
                    {
                        listUser.Clear();
                    });

                    int k = 0;
                    while (k < splitListUser.Length)
                    {
                        int id = int.Parse(splitListUser[k++]);
                        String name = splitListUser[k++];

                        User user = new User(id, name);
                        Dispatcher.Invoke(() =>
                        {
                            listUser.Add(user);
                        });
                    }
                    break;

                case 10:
                    ViewScreenWindow viewScreenWindow = ListViewScreenWindow.Where(vs => { return vs.UserID == command[1]; }).FirstOrDefault();

                    //byte[] bytew = command.Skip(2).Take(4).ToArray();
                    //byte[] byteh = command.Skip(6).Take(4).ToArray();
                    //double width = BitConverter.ToDouble(bytew, 0);
                    //double height = BitConverter.ToDouble(byteh, 0);

                    int x = BitConverter.ToInt32(command.Skip(2).Take(4).ToArray(), 0);
                    int y = BitConverter.ToInt32(command.Skip(6).Take(4).ToArray(), 0);

                    BitmapSource screenImage = BitmapSourceFromArray(command.Skip(10).ToArray());
                    screenImage.Freeze();

                    viewScreenWindow.SetImage(screenImage, x, y);

                    //Console.WriteLine("new" + new Random().Next(100));
                    break;
            }
        }

        private BitmapSource BitmapSourceFromArray(byte[] data)
        {
            JpegBitmapDecoder jpegBitmapDecoder = new JpegBitmapDecoder(new MemoryStream(data), BitmapCreateOptions.None, BitmapCacheOption.Default);
            return jpegBitmapDecoder.Frames[0];
        }

        private BitmapSource BitmapSourceFromArray(byte[] pixels, int width, int height)
        {
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * (bitmap.Format.BitsPerPixel / 8), 0);

            return bitmap;
        }

        private static BitmapImage ConvertByteArrayToBitmap(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public void SendIPPort()
        {
            udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 3, 1 });
        }

        private void GetIPPortList()
        {
            udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 4 });
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
            udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 9 });
        }

        private void BtnRefreshUserList_Click(object sender, RoutedEventArgs e)
        {
            RefreshUserList();
            GetIPPortList();
        }

        private void ShowViewScreenWindow(User user)
        {
            ViewScreenWindow viewScreenWindow = ListViewScreenWindow.Where(win => { return win.UserID == user.ID; }).FirstOrDefault();

            if (viewScreenWindow == null)
            {
                viewScreenWindow = new ViewScreenWindow(user.ID);
                viewScreenWindow.SetCloseAction(() =>
                {
                    ListViewScreenWindow.Remove(viewScreenWindow);
                });

                ListViewScreenWindow.Add(viewScreenWindow);
            }

            viewScreenWindow.Show();
        }

        private void ShowChatWindow(IPPort iPPort, String message = null)
        {
            ChatWindow chatWindow = listChatWindow.Where((chatwin) => { return chatwin.UserID == iPPort.UserId; }).FirstOrDefault();

            if (chatWindow == null)
            {
                chatWindow = new ChatWindow(mess =>
                {
                    SendChatMess(mess, iPPort.IP, iPPort.Port);
                },
                   iPPort.UserId);

                chatWindow.SetCloseAction(() =>
                {
                    listChatWindow.Remove(chatWindow);
                });

                listChatWindow.Add(chatWindow);
            }

            chatWindow.Dispatcher.Invoke(() =>
            {
                chatWindow.Show();
            });

            if (message != null)
            {
                chatWindow.Dispatcher.Invoke(() =>
                {
                    chatWindow.PushMessage(message, false);
                });
            }
        }

        private void BtnChat_Click(object sender, RoutedEventArgs e)
        {
            if (listViewUsers.SelectedIndex >= 0)
            {
                User user = listViewUsers.SelectedItem as User;
                IPPort ipport = listIPPort.Where(ipp => { return ipp.UserId == user.ID; }).FirstOrDefault();
                //int userid = ipport.UserId;

                //udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 7, (byte)userid });

                //for (int i = 0; i < 2; i++)
                //{
                //    udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 0 });
                //}

                EstablishConnection(user, ipport);
                ShowChatWindow(ipport);
            }
        }

        private void setUserStatus(User user)
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

        private void SendChatMess(String text, String clientIP, int port)
        {
            if (clientIP == null) return;

            byte[] textbyte = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] sendbyte = new byte[textbyte.Length + 1];

            sendbyte[0] = 8;

            Array.Copy(textbyte, 0, sendbyte, 1, textbyte.Length);

            udpProtocol.UdpSocketSend(clientIP, port, sendbyte);
        }

        private void enableControlButton(Boolean isEnable)
        {
            btnChat.IsEnabled = btnFileExplorer.IsEnabled =
            btnMessageBox.IsEnabled = btnRestart.IsEnabled =
            btnShutdown.IsEnabled = btnViewScreen.IsEnabled =
            btnChangeName.IsEnabled = isEnable;
        }

        private void EstablishConnection(User user, IPPort iPPort)
        {
            //IPPort iPPort = listIPPort.Where((ipp) => { return ipp.UserId == user.ID; }).FirstOrDefault();

            byte[] data = new byte[] { 7, (byte)user.ID };
            udpProtocol.UdpSocketSend(serverIP, serverPort, data);

            System.Threading.Thread.Sleep(50);
            for (int i = 0; i < 5; i++)
            {
                udpProtocol.UdpSocketSend(iPPort.IP, iPPort.Port, new byte[] { 0 });
            }
        }

        private void ListViewUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewUsers.SelectedIndex < 0)
            {
                enableControlButton(false);
                setUserStatus(null);
            }
            else
            {
                enableControlButton(true);
                setUserStatus(listViewUsers.SelectedItem as User);
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

            EstablishConnection(user, ipport);

            udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 10 });

            ShowViewScreenWindow(user);
        }

        private void BtnChangeName_Click(object sender, RoutedEventArgs e)
        {
            User user = listViewUsers.SelectedItem as User;
            ChangeNameWindow changeNameWindow = new ChangeNameWindow(user.Name, str => {
                byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(str);
                byte[] sendData = new byte[] { 2, (byte)user.ID }.Concat(nameBytes).ToArray();
                udpProtocol.UdpSocketSend(serverIP, serverPort, sendData);
                RefreshUserList();
                GetIPPortList();
            });

            changeNameWindow.ShowDialog();
        }
    }
}
