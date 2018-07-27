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
    public abstract class Uploader : IDisposable
    {
        public event EventHandler UploadStarted;
        public event EventHandler<System.Net.UploadProgressChangedEventArgs> UploadProgressChanged;
        public event EventHandler<AsyncCompletedEventArgs> UploadCompleted;

        protected virtual void OnUploadStarted(object sender, EventArgs e)
        {
            if (null != this.UploadStarted)
                this.UploadStarted(sender, e);
        }
        protected virtual void OnUploadProgressChanged(object sender, System.Net.UploadProgressChangedEventArgs e)
        {
            if (null != this.UploadProgressChanged)
                this.UploadProgressChanged(sender, e);
        }
        protected virtual void OnUploadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (null != this.UploadCompleted)
                this.UploadCompleted(sender, e);
        }

        protected WebClient WebClient { get; set; }

        public Guid Id { get; set; }
        public Server Server { get; private set; }
        public FileInfo FileFTP { get; private set; }

        private const string PART_EXTENSION = "part";

        public long? FileFTP_Size { get { return this.GetResourceSize(); } }
        
        public Uri UriUpload { get; private set; }
        public Uri UriUploadPartial { get; private set; }

        public Exception ErrorAsync { get; protected set; }

        private object m_SyncUploadLock = new object();

        public Uploader(FileInfo file)
        {
            Debug.Assert(null != file);

            this.Id = Guid.NewGuid();

            this.Server = file.Server;
            this.FileFTP = file;

            this.UriUpload = new Uri(string.Format(@"{0}:{1}/{2}/{3}", this.Server.Address, this.Server.Port, this.FileFTP.Directory.FullPath, this.FileFTP.Name));
            this.UriUploadPartial = new Uri(string.Format(@"{0}:{1}/{2}/{3}.{4}", this.Server.Address, this.Server.Port, this.FileFTP.Directory.FullPath, this.FileFTP.Name, PART_EXTENSION));

            this.WebClient = new WebClient();
            this.WebClient.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);
            this.WebClient.UploadProgressChanged += client_UploadProgressChanged;
            this.WebClient.UploadFileCompleted += client_UploadFileCompleted;
            this.WebClient.UploadDataCompleted += client_UploadDataCompleted;
        }

        ~Uploader()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this.WebClient)
                {
                    this.WebClient.UploadDataCompleted -= client_UploadDataCompleted;
                    this.WebClient.UploadFileCompleted -= client_UploadFileCompleted;
                    this.WebClient.UploadProgressChanged -= client_UploadProgressChanged;
                    this.WebClient = null;
                }
            }
        }

        protected abstract long? GetResourceSize();

        protected abstract void DoUpload(Uri uriUpload);
        protected abstract void DoUploadAsync(Uri uriUpload);

        public void StartUpload(bool async = false)
        {
            this.FileFTP.Directory.CreateWithSubfolders();

            if (async)
            {
                this.OnUploadStarted(this, EventArgs.Empty);
                this.DoUploadAsync(this.UriUploadPartial);
            }
            else
            {
                lock (m_SyncUploadLock)
	            {
                    this.OnUploadStarted(this, EventArgs.Empty);
                    this.OnUploadProgressChanged(this, null);
                    this.DoUpload(this.UriUploadPartial);
                    this.ConcludePartialFile(new AsyncCompletedEventArgs(error: null, cancelled: false, userState: null));
                    this.OnUploadCompleted(this, null);
                }
            }
        }

        public void StopUpload()
        {
            // Calls to stop when uploading sync, wait until the end
            lock (m_SyncUploadLock)
            {
                if (null != this.WebClient)
                    this.WebClient.CancelAsync();
            }
        }

        void client_UploadProgressChanged(object sender, System.Net.UploadProgressChangedEventArgs e)
        {
            this.OnUploadProgressChanged(this, e);
        }

        void client_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            this.ConcludePartialFile(e);
            this.OnUploadCompleted(this, e);
        }

        void client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            this.ConcludePartialFile(e);
            this.OnUploadCompleted(this, e);
        }

        private void ConcludePartialFile(AsyncCompletedEventArgs e)
        {
            string l_FileNamePartial = string.Format("{0}.{1}", this.FileFTP.Name, PART_EXTENSION);
            FileInfo ftpFilePartial = new FileInfo(this.FileFTP.Directory, l_FileNamePartial);
            if (ftpFilePartial.Exists)
            {
                if (e != null && e.Cancelled)
                {
                    bool isDeleted = ftpFilePartial.Delete();
                    if(!isDeleted)
                        System.Diagnostics.Trace.TraceWarning("File was partially uploaded but not deleted. Garbage is remaining at '{0}'", this.UriUploadPartial.ToString());
                }
                else
                {
                    bool isRenamed = ftpFilePartial.RenameTo(this.FileFTP.Name);
                    if (!isRenamed)
                        throw new FtpException(string.Format("File '{0}' was partially uploaded but not renamed", this.FileFTP.Name));
                }
            }
        }
    }
}
