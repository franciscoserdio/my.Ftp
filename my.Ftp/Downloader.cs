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
    public abstract class Downloader : IDisposable
    {
        public event EventHandler DownloadStarted;
        public event EventHandler<System.Net.DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;

        protected virtual void OnDownloadAsyncStarted(object sender, EventArgs e)
        {
            if (null != this.DownloadStarted)
                this.DownloadStarted(sender, e);
        }
        protected virtual void OnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (null != this.DownloadProgressChanged)
                this.DownloadProgressChanged(sender, e);
        }
        protected virtual void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (null != this.DownloadCompleted)
                this.DownloadCompleted(sender, e);
        }

        protected WebClient WebClient { get; set; }

        public Guid Id { get; set; }
        public Server Server { get; private set; }
        public FileInfo FileFTP { get; private set; }

        public long? FileFTP_Size { get; private set; }

        public Uri UriDownload { get; private set; }

        public Exception ErrorAsync { get; protected set; }

        private object m_SyncDownloadLock = new object();

        public Downloader(FileInfo file)
        {
            Debug.Assert(null != file);

            this.Id = Guid.NewGuid();

            this.Server = file.Server;
            this.FileFTP = file;

            this.UriDownload = new Uri(string.Format(@"{0}:{1}/{2}/{3}", this.Server.Address, this.Server.Port, this.FileFTP.Directory.FullPath, this.FileFTP.Name));

            this.WebClient = new WebClient();
            this.WebClient.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);
            this.WebClient.DownloadProgressChanged += client_DownloadProgressChanged;
            this.WebClient.DownloadDataCompleted += client_DownloadDataCompleted;
            this.WebClient.DownloadFileCompleted += client_DownloadFileCompleted;

            this.FileFTP_Size = File.GetSize(this.FileFTP.Directory, this.FileFTP.Name);
        }
        ~Downloader()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this.WebClient)
                {
                    this.WebClient.DownloadFileCompleted -= client_DownloadFileCompleted;
                    this.WebClient.DownloadDataCompleted -= client_DownloadDataCompleted;
                    this.WebClient.DownloadProgressChanged -= client_DownloadProgressChanged;
                    this.WebClient = null;
                }
            }
        }

        protected abstract void DoDownload(Uri uriDownload);
        protected abstract void DoDownloadAsync(Uri uriDownload);

        public void StartDownload(bool async = false)
        {
            if (async)
            {
                this.OnDownloadAsyncStarted(this, EventArgs.Empty);
                this.DoDownloadAsync(this.UriDownload);
            }
            else
            {
                lock (m_SyncDownloadLock)
	            {
                    this.DoDownload(this.UriDownload);
	            }
            }
        }

        public void StopDownload()
        {
            // Calls to stop when downloading sync, wait until the end
            lock (m_SyncDownloadLock)
            {
                if (null != this.WebClient)
                    this.WebClient.CancelAsync();
            }
        }

        void client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.OnDownloadProgressChanged(this, e);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.OnDownloadCompleted(this, e);
        }

        void client_DownloadDataCompleted(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            this.OnDownloadCompleted(this, e);
        }

        public override bool Equals(object obj)
        {
            Downloader objCasted = obj as Downloader;
            if (null == objCasted)
                return false;

            return this.GetHashCode().Equals(objCasted.GetHashCode());
        }
        public override int GetHashCode()
        {
            return this.UriDownload.GetHashCode();
        }
    }
}
