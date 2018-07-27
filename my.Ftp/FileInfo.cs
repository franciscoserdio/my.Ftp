using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Diagnostics;

namespace my.Ftp
{
    public class FileInfo
    {
        public Server Server { get; private set; }
        public DirectoryInfo Directory { get; private set; }
        public string Name { get; private set; }

        public string FullName { get { return string.Format("{0}/{1}", this.Directory.FullPath, this.Name); } }
        private Uri FullAddress { get { return new Uri(string.Format(@"{0}/{1}", this.Server.Address, this.FullName)); } }

        public FileInfo(DirectoryInfo directory, string name)
        {
            Debug.Assert(null != directory);
            Debug.Assert(!string.IsNullOrEmpty(name));

            this.Server = directory.Server;
            this.Directory = directory;
            this.Name = name;

            this.GetDetails();
        }

        public bool Exists { get; set; }

        public long? Size { get; set; }

        public bool RenameTo(string newName)
        {
            try
            {
                FtpWebRequest l_Request = FtpWebRequest.Create(this.FullAddress) as FtpWebRequest;
                l_Request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);
                l_Request.Method = WebRequestMethods.Ftp.Rename;
                l_Request.RenameTo = newName;

                using (FtpWebResponse l_Response = l_Request.GetResponse() as FtpWebResponse)
                {
                    if (l_Response.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        this.Name = newName;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                FtpWebRequest l_Request = FtpWebRequest.Create(this.FullAddress) as FtpWebRequest;
                l_Request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);
                l_Request.Method = WebRequestMethods.Ftp.DeleteFile;

                using (FtpWebResponse l_Response = l_Request.GetResponse() as FtpWebResponse)
                {
                    if (l_Response.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        this.Exists = false;
                        this.Size = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = ex.Response as FtpWebResponse;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                    return false;
                }
                else
                {
                    throw;
                }
            } 
        }

        private void GetDetails()
        {
            try
            {
                FtpWebRequest l_Request = FtpWebRequest.Create(this.FullAddress) as FtpWebRequest;
                l_Request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);
                l_Request.Method = WebRequestMethods.Ftp.GetFileSize;

                using (FtpWebResponse l_Response = l_Request.GetResponse() as FtpWebResponse)
                {
                    if (l_Response.StatusCode == FtpStatusCode.FileStatus)
                    {
                        this.Exists = true;
                        this.Size = (null != l_Response) ? l_Response.ContentLength : new Nullable<long>();
                    }
                    else
                    {
                        throw new Exception(l_Response.StatusCode.ToString());
                    }
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = ex.Response as FtpWebResponse;
                if (null == response || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                    this.Exists = false;
                    this.Size = null;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public override bool Equals(object obj)
        {
            FileInfo objCasted = obj as FileInfo;
            if (null == objCasted)
                return false;

            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this.FullAddress.GetHashCode();
        }
    }
}
