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
    public static class DirectoryInfoExtensor
    {
        public static UploaderFile[] CreateUploaders(
            this DirectoryInfo directoryHD,
            my.Ftp.DirectoryInfo directoryFTP,
            string searchPattern,
            bool recursive = false, 
            CancellationTokenSource cancellation = null)
        {
            List<UploaderFile> uploaders = new List<UploaderFile>();
            if (directoryHD.Exists)
            {
                foreach (FileInfo fileHD in directoryHD.GetFiles(searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    UploaderFile uploderFile = fileHD.CreateUploader(directoryFTP);
                    if (uploderFile != null)
                        uploaders.Add(uploderFile);

                    if (null != cancellation && cancellation.IsCancellationRequested)
                        throw new OperationCanceledException();
                }
            }

            return uploaders.ToArray();
        }
    }
}

