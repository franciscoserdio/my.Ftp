using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my.Ftp
{
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public Credentials(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }
    }
}
