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
    public class UploaderData : Uploader
    {
        public byte[] ToUploadData { get; set; }
        public byte[] UploadedData { get; set; }

        public UploaderData(FileInfo file_FTP, byte[] toUploadData)
            : base(file_FTP)
        {
            Debug.Assert(null != toUploadData);

            this.ToUploadData = toUploadData;
        }

        protected override long? GetResourceSize()
        {
            return this.ToUploadData.Length;
        }

        protected override void DoUpload(Uri uriUpload)
        {
            this.UploadedData = this.WebClient.UploadData(uriUpload, this.ToUploadData);
        }

        protected override void DoUploadAsync(Uri uriUpload)
        {
            this.WebClient.UploadDataAsync(uriUpload, this.ToUploadData);
        }

        protected override void OnUploadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (null != e)
            {
                this.UploadedData = (!e.Cancelled) ? (e as UploadDataCompletedEventArgs).Result : null;

                if (null != e.Error)
                    this.ErrorAsync = e.Error;
            }
            base.OnUploadCompleted(sender, e);
        }
    }
}
