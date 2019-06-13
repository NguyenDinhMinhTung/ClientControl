using ClientControl.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientControl
{
    class ChatWindowManager
    {
        private List<ChatWindow> listChatWindow;
        private UDPProtocol udpProtocol;

        public ChatWindowManager(UDPProtocol udpProtocol)
        {
            this.udpProtocol = udpProtocol;
            listChatWindow = new List<ChatWindow>();
        }

        public void ShowChatWindow(IPPort iPPort, String message = null)
        {
            ChatWindow chatWindow = listChatWindow.Where((chatwin) => { return chatwin.UserID == iPPort.UserId; }).FirstOrDefault();

            if (chatWindow == null)
            {
                chatWindow = new ChatWindow(mess =>
                {
                    SendChatMess(mess, iPPort);
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

        private void SendChatMess(String text, IPPort ipPort)
        {
            byte[] textbyte = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] sendbyte = null;
            sendbyte = new byte[] { 8, 1, (byte)ipPort.UserId }.Concat(textbyte).ToArray();
            udpProtocol.UdpSocketSend(ipPort.IP, ipPort.Port, sendbyte);
        }
    }
}
