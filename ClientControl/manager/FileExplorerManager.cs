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
    }
}
