using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace my.Ftp
{
    [Serializable]
    public class Server
    {
        public Server()
        {
            this.Port = 21;
            this.UseSSL = false;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }
        public bool UseSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2}", this.Name, this.Address, this.Port);
        }
    }
}
