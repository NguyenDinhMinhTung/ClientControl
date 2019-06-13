using ClientControl.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientControl
{
    class ViewScreenManager
    {
        private UDPProtocol udpProtocol;
        List<ViewScreenWindow> ListViewScreenWindow;


        public ViewScreenManager(UDPProtocol udpProtocol)
        {
            this.udpProtocol = udpProtocol;
            ListViewScreenWindow = new List<ViewScreenWindow>();
        }

        public void ShowViewScreenWindow(IPPort ipport)
        {
            ViewScreenWindow viewScreenWindow = ListViewScreenWindow.Where(win => { return win.UserID == ipport.UserId; }).FirstOrDefault();

            if (viewScreenWindow == null)
            {
                udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 10, 1, (byte)ipport.UserId, 1 });
                viewScreenWindow = new ViewScreenWindow(ipport.UserId);
                viewScreenWindow.SetCloseAction(() =>
                {
                    ListViewScreenWindow.Remove(viewScreenWindow);
                    udpProtocol.UdpSocketSend(ipport.IP, ipport.Port, new byte[] { 10, 1, (byte)ipport.UserId, 0 });
                });

                ListViewScreenWindow.Add(viewScreenWindow);
            }

            viewScreenWindow.Show();
        }

        public void SetImage(int userid, BitmapSource image, int x, int y)
        {
            ViewScreenWindow viewScreenWindow = ListViewScreenWindow.Where(vs => { return vs.UserID == userid; }).FirstOrDefault();
            if (viewScreenWindow != null)
            {
                viewScreenWindow.SetImage(image, x, y);
            }
        }
    }
}
