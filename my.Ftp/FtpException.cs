using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace my.Ftp
{
    [Serializable]
    public class FtpException : Exception
    {
        public FtpException() : base() { }
        
        public FtpException(string message) : base(message) { }
        
        [SecuritySafeCritical]
        protected FtpException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public FtpException(string message, Exception innerException) : base(message, innerException) { }
    }
}
