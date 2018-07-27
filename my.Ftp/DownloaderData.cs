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
    public class DownloaderData : Downloader
    {
        public byte[] DownloadedData { get; set; }

        public DownloaderData(Server ftpServer, string ftpFolder, string fileName)
            :this(new FileInfo(new DirectoryInfo(ftpServer, ftpFolder), fileName))
        { 
        }

        public DownloaderData(FileInfo fileFTP)
            : base(fileFTP)
        {
        }

        protected override void DoDownload(Uri uriDownload)
        {
            this.DownloadedData = this.WebClient.DownloadData(uriDownload);
        }

        protected override void DoDownloadAsync(Uri uriDownload)
        {
            this.WebClient.DownloadDataAsync(uriDownload);
        }

        protected override void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.DownloadedData = !e.Cancelled ? (e as DownloadDataCompletedEventArgs).Result : null;
            
            if (null != e.Error)
                this.ErrorAsync = e.Error;

            base.OnDownloadCompleted(sender, e);
        }
    }
}
