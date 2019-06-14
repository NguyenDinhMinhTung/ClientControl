using ClientControl.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientControl.manager
{
    class FileExplorerManager
    {
        private UDPProtocol udpProtocol;

        private List<FileExplorerWindow> listFileExplorerWindow;

        public FileExplorerManager(UDPProtocol udpProtocol)
        {
            this.udpProtocol = udpProtocol;
            listFileExplorerWindow = new List<FileExplorerWindow>();
        }

        public void ShowFileExplorerWindow(IPPort ipport)
        {
            FileExplorerWindow fileExplorerWindow = listFileExplorerWindow.Where(win => { return win.UserID == ipport.UserId; }).FirstOrDefault();

            if (fileExplorerWindow == null)
            {

            }
        }
    }
}
