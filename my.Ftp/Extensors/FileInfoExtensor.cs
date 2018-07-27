using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using my.Ftp;

namespace System.IO
{
    public static class FileInfoExtensor
    {
        public static UploaderFile CreateUploader(this FileInfo fileHD, my.Ftp.DirectoryInfo fileFTP)
        {
            if (fileHD.Exists)
            {
                return new UploaderFile(
                    fileFTP: (my.Ftp.FileInfo)new my.Ftp.FileInfo((my.Ftp.DirectoryInfo)fileFTP, (string)fileHD.Name),
                    fileHD: (System.IO.FileInfo)fileHD);
            }
            else
            {
                return null;
            }
        }
    }
}

