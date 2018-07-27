using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using my.Ftp;
using System.Threading;

namespace System.IO
{
    public static class IEnumerableFileInfoExtensor
    {
        public static UploaderFile[] CreateUploaders(this IEnumerable<FileInfo> filesHD, my.Ftp.DirectoryInfo fileFTP)
        {
            List<UploaderFile> uploaders = new List<UploaderFile>();
            foreach (FileInfo fleHD in filesHD)
            {
                if (fleHD.Exists)
                {
                    uploaders.Add(
                        new UploaderFile(
                        fileFTP: (my.Ftp.FileInfo)new my.Ftp.FileInfo((my.Ftp.DirectoryInfo)fileFTP, (string)fleHD.Name),
                        fileHD: (System.IO.FileInfo)fleHD)
                        );
                }
            }
            return uploaders.ToArray();
        }
    }
}

