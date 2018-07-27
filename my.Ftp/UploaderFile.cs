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
    public class UploaderFile : Uploader
    {
        public System.IO.FileInfo FileHD { get; set; }

        public UploaderFile(Server ftpServer, string ftpFolder, string fileName, System.IO.FileInfo fileHD)
            :this(new FileInfo(new DirectoryInfo(ftpServer, ftpFolder), fileName), fileHD)
        { 
        }

        public UploaderFile(FileInfo fileFTP, System.IO.FileInfo fileHD)
            :base(fileFTP)
        {
            Debug.Assert(null != fileFTP);
            Debug.Assert(null != fileHD);

            if (!fileHD.Exists)
                throw new FileNotFoundException();

            this.FileHD = fileHD;
        }

        protected override long? GetResourceSize()
        {
            return this.FileHD.Length;
        }

        protected override void DoUpload(Uri uriUpload)
        {
            this.WebClient.UploadFile(uriUpload, this.FileHD.FullName);
        }

        protected override void DoUploadAsync(Uri uriUpload)
        {
            this.WebClient.UploadFileAsync(uriUpload, this.FileHD.FullName);
        }

        protected override void OnUploadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (null != e && null != e.Error)
                this.ErrorAsync = e.Error;

            base.OnUploadCompleted(sender, e);
        }
    }
}
