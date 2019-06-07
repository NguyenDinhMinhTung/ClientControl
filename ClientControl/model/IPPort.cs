using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientControl.model
{
    class IPPort
    {
        private int id;
        private int userid;
        private String ip;
        private int port;
        private DateTime? dateTime;

        public IPPort(int id, int userid, String ip, int port, DateTime? dateTime = null)
        {
            this.ID = id;
            this.UserId = userid;
            this.IP = ip;
            this.Port = port;
            this.DateTime = dateTime;
        }
        public DateTime? DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public String IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public int UserId
        {
            get { return userid; }
            set { userid = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
