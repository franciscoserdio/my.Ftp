using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace my.Ftp
{
    public class DownloaderFile : Downloader
    {
        public System.IO.FileInfo FileHD { get; private set; }

        public string FileHD_FullName { get { return this.FileHD.FullName; } }
        public string FileHD_FullName_part { get { return string.Format("{0}.part", this.FileHD_FullName); } }

        public DownloaderFile(Server ftpServer, string ftpFolder, string fileName, System.IO.FileInfo fileHD)
            :this(new FileInfo(new DirectoryInfo(ftpServer, ftpFolder), fileName), fileHD)
        { 
        }

        public DownloaderFile(FileInfo fileFTP, System.IO.FileInfo fileHD)
            : base(fileFTP)
        {
            Debug.Assert(null != fileHD);

            this.FileHD = fileHD;
        }

        protected override void DoDownload(Uri uriDownload)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this.FileHD.FullName));
            this.WebClient.DownloadFile(uriDownload, this.FileHD_FullName);
        }

        protected override void DoDownloadAsync(Uri uriDownload)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this.FileHD.FullName));
            this.WebClient.DownloadFileAsync(uriDownload, this.FileHD_FullName_part);
        }

        protected override void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Not cancelled -> Remove the .part extension
            if (!e.Cancelled && System.IO.File.Exists(this.FileHD_FullName_part))
            {
                if (System.IO.File.Exists(this.FileHD_FullName))
                    System.IO.File.Delete(this.FileHD_FullName);

                System.IO.File.Move(this.FileHD_FullName_part, this.FileHD_FullName);
            }

            // Cancelled -> Remove the garbage
            if (e.Cancelled && System.IO.File.Exists(this.FileHD_FullName_part))
            {
                try
                {
                    System.IO.File.Delete(this.FileHD_FullName_part);
                }
                catch (Exception)
                {
                    // Silent the exception, trace that garbage is around
                    System.Diagnostics.Trace.TraceWarning("File was partially downloaded but not deleted. Garbage is remaining at '{0}'", this.FileHD_FullName_part);
                }
            }

            if (null != e.Error)
                this.ErrorAsync = e.Error;

            base.OnDownloadCompleted(sender, e);
        }
    }
}
