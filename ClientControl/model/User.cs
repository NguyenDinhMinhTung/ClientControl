using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientControl.model
{
    class User
    {
        private int id;
        private String name;

        public User(int id, String name)
        {
            this.ID = id;
            this.Name = name;
        }
        public int ID { get { return id; } set { id = value; } }
        public String Name { get { return name; } set { name = value; } }
    }
}
