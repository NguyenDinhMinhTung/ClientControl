using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientControl
{
    class UDPProtocol
    {
        private System.Net.Sockets.UdpClient udpClient;

        private int localPort;
        public UDPProtocol(int localPort)
        {
            this.localPort = localPort;

            System.Net.IPAddress localIpAddress = System.Net.IPAddress.Parse(GetLocalIPAddress());

            System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(localIpAddress, localPort);

            udpClient = new System.Net.Sockets.UdpClient(localEP);
        }

        public void UdpSocketSend(IPEndPoint iPEndPoint, byte[] data)
        {
            udpClient.Send(data, data.Length, iPEndPoint);
        }

        public void UdpSocketSend(String host, int port, byte[] data)
        {
            udpClient.Send(data, data.Length, host, port);
        }

        public void UdpSocketReceiveStart(Action<IPEndPoint, byte[]> action)
        {
            System.Net.IPEndPoint remoteEP = null;

            Thread socketReceiveThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] receiveBytes = udpClient.Receive(ref remoteEP);
                    Console.WriteLine("{0} {1}", remoteEP.Address, remoteEP.Port);
                    action(remoteEP, receiveBytes);
                    //Environment.Exit(0);
                }
            });

            socketReceiveThread.IsBackground = true;
            socketReceiveThread.Start();


            //udpClient.Close();
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void Close()
        {
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
