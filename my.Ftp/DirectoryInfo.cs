using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.Cache;

namespace my.Ftp
{
    public class DirectoryInfo
    {
        public Server Server { get; set; }

        public string FullPath { get; set; }
        public string Name { get { return this.FullPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last(); } }

        public DirectoryInfo(Server ftpServer, string fullName)
        {
            Debug.Assert(null != ftpServer);
            Debug.Assert(!string.IsNullOrEmpty(fullName));

            fullName = fullName.ToValidPath_FTP();

            if(fullName.StartsWith("/"))
                fullName = fullName.Substring(startIndex: 1);

            // Ending with a "/" is neccesarry to have the property "Exists" working, otherwise, it will return true always
            if(!fullName.EndsWith("/"))
                fullName = string.Format("{0}/", fullName);

            fullName = fullName.Replace(@"\", "/");

            this.Server = ftpServer;
            this.FullPath = fullName;
        }

        public DirectoryInfo(DirectoryInfo parent, string name)
            : this(ftpServer: parent.Server, fullName: parent.FullPath + name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
        }

        private string Address
        {
            get { return string.Format(@"{0}/{1}", this.Server.Address, this.FullPath); }
        }

        public bool Exists
        {
            get 
            {
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(this.Address);
                    request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);

                    request.Method = WebRequestMethods.Ftp.ListDirectory;

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        return true;
                    }
                }
                catch (WebException ex)
                {
                    if ((ex.Response != null) && (ex.Response as FtpWebResponse).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public bool Create()
        {
            // Check if existing
            if (this.Exists)
                return true;

            // Create
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(this.Address);
            request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);

            request.Method = WebRequestMethods.Ftp.MakeDirectory;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                return (FtpStatusCode.PathnameCreated == response.StatusCode);
        }
        public bool CreateWithSubfolders()
        { 
            List<string> l_FullPaths = new List<string>();
            Queue<string> l_DirectoryNames = new Queue<string>(this.FullPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

            if (0 != l_DirectoryNames.Count)
            {
                string l_currentFolder = l_DirectoryNames.Dequeue();
                l_FullPaths.Add(l_currentFolder);
            }

            while (0 != l_DirectoryNames.Count)
            {
                string l_currentFolder = l_DirectoryNames.Dequeue();
                l_FullPaths.Add(string.Format("{0}{1}{2}", l_FullPaths.Last(), Path.DirectorySeparatorChar, l_currentFolder));
            }

            bool isCreated = false;
            foreach (string l_FullPath in l_FullPaths)
            {
                isCreated = Directory.Create(this.Server, l_FullPath);
                if (!isCreated)
                    return false;
            }
            return true;
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            if (!this.Exists)
                yield break;

            IEnumerable<string> l_Lines = this.GetListing().ToArray();

            foreach (string l_Line in l_Lines)
            {
                string[] lineParts = l_Line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                bool directory = ((null != lineParts) && (0 != lineParts.Length) && (lineParts.First().StartsWith("d")));
                if (!directory)
                {
                    // TODO -> (This assumes from 8 on, it is the directory or file name
                    string fileName = string.Join(" ", lineParts, 8, lineParts.Length - 8);
                    yield return new FileInfo(this, fileName);
                    // yield return new FtpFileInfo(this, lineParts.Last());
                }
            }
        }
        public IEnumerable<FileInfo> GetFiles(string p_SearchPattern, SearchOption p_SearchOption = SearchOption.Exact, StringComparison p_StringComparison = StringComparison.Ordinal)
        {
            IEnumerable<FileInfo> l_Files = this.GetFiles().ToArray();
            switch (p_SearchOption)
            {
                case SearchOption.Exact:
                    foreach (FileInfo l_File in l_Files)
                        if (l_File.Name.Equals(p_SearchPattern, p_StringComparison))
                            yield return l_File;

                    yield break;

                case SearchOption.StartsWith:
                    foreach (FileInfo l_File in l_Files)
                        if (l_File.Name.StartsWith(p_SearchPattern, p_StringComparison))
                            yield return l_File;

                    yield break;
                
                case SearchOption.Contains:
                    foreach (FileInfo l_File in l_Files)
                        if (0 <= l_File.Name.IndexOf(p_SearchPattern, p_StringComparison))
                            yield return l_File;

                    yield break;

                case SearchOption.EndsWith:
                    foreach (FileInfo l_File in l_Files)
                        if (l_File.Name.EndsWith(p_SearchPattern, p_StringComparison))
                            yield return l_File;

                    yield break;

                default:
                    throw new ArgumentException();
            }
        }

        public bool ContainsFile(string fileName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileName));
            
            if(!this.Exists)
                return false;

            return 0 != this.GetFiles(fileName, SearchOption.Exact).Count();
        }

        public DirectoryInfo GetFolderChild(string p_FolderName)
        {
            return new DirectoryInfo(this, p_FolderName);
        }
        public IEnumerable<DirectoryInfo> GetFolders()
        {
            if (!this.Exists)
                yield break;

            List<DirectoryInfo> l_Folders = new List<DirectoryInfo>();

            string[] l_Lines = this.GetListing().ToArray();
            foreach (string l_Line in l_Lines)
            {
                string[] lineParts = l_Line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                bool directory = ((null != lineParts) && (0 != lineParts.Length) && (lineParts.First().StartsWith("d")));
                if (directory)
                {
                    // TODO -> (This assumes from 8 on, it is the directory or file name
                    string directoryName = string.Join(" ", lineParts, 8, lineParts.Length - 8);
                    yield return new DirectoryInfo(this, directoryName);
                    // yield return new FtpDirectoryInfo(this, lineParts.Last());
                }
            }
        }
        public IEnumerable<DirectoryInfo> GetFolders(string p_SearchPattern, SearchOption p_SearchOption = SearchOption.Exact, StringComparison p_StringComparison = StringComparison.Ordinal)
        {
            IEnumerable<DirectoryInfo> l_Folders = this.GetFolders().ToArray();
            switch (p_SearchOption)
            {
                case SearchOption.Exact:
                    foreach (DirectoryInfo l_Folder in l_Folders)
                        if (l_Folder.Name.Equals(p_SearchPattern, p_StringComparison))
                            yield return l_Folder;

                    yield break;

                case SearchOption.StartsWith:
                    foreach (DirectoryInfo l_Folder in l_Folders)
                        if (l_Folder.Name.StartsWith(p_SearchPattern, p_StringComparison))
                            yield return l_Folder;

                    yield break;

                case SearchOption.Contains:
                    Trace.TraceWarning("FtpSearchOption.Contains ignores the StringComparison parameter");
                    foreach (DirectoryInfo l_Folder in l_Folders)
                        if (0 <= l_Folder.Name.IndexOf(p_SearchPattern, p_StringComparison))
                            yield return l_Folder;

                    yield break;

                case SearchOption.EndsWith:
                    foreach (DirectoryInfo l_Folder in l_Folders)
                        if (l_Folder.Name.EndsWith(p_SearchPattern, p_StringComparison))
                            yield return l_Folder;

                    yield break;

                default:
                    throw new ArgumentException();
            }
        }

        private IEnumerable<string> GetListing()
        {
            // GetListing fails sometimes, and instead of returning
            // {
            // -rw-rw-r--    1 1001       ftpgroup          142 Feb 20  2017 010k11_ACCAMEasyADV.v3.lic
            // -rw-rw-r--    1 1001       ftpgroup          142 May 10  2017 010k11_ACCAMEasyADV.v4.lic
            // }
            // returns the directory names, such
            // {
            // VIA0SB
            // VIA0QJ
            // }

            Func<string[]> try_listing = new Func<string[]>(
                ()=>
                {
                    string[] attempt_1 = this._GetListing().ToArray();
                    string[] attempt_2 = this._GetListing().ToArray();

                    if (0 == attempt_1.Length && 0 == attempt_2.Length)
                        return attempt_1;

                    string attempt_1_line_1 = attempt_1.First();
                    string attempt_2_line_1 = attempt_2.First();

                    string[] split_1_1 = attempt_1_line_1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] split_2_1 = attempt_2_line_1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (8 < split_1_1.Length)
                    {
                        Trace.TraceInformation("my.ftp 'GetListing' --> successfull at first attempt");
                        return attempt_1;
                    }

                    if (8 < split_2_1.Length)
                    {
                        Trace.TraceWarning("my.ftp 'GetListing' --> corrected at second attempt");
                        return attempt_2;
                    }

                    Trace.TraceError("my.ftp 'GetListing' --> unable to be corrected at second attempt");
                    throw new FtpException();
                }
            );

            try
            {
                try
                {
                    Trace.TraceInformation("my.ftp 'GetListing' --> first try with 2 attempts");
                    return try_listing();
                }
                catch (FtpException)
                {
                    Trace.TraceInformation("my.ftp 'GetListing' --> second try with 2 attempts");
                    return try_listing();
                }
            }
            catch
            {
                Trace.TraceError("my.ftp 'GetListing' --> two tries with 2 attempts failed");
                throw;
            }
        }
        private IEnumerable<string> _GetListing()
        {
            if (!this.Exists)
                yield break;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(this.Address);
            request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);

            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            List<string> l_Lines = new List<string>();

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string line = string.Empty;
                        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                            yield return line;
                    }
                }
            }
}

        public IEnumerable<Downloader> CreateFileDownloaders(string localBasePath, bool recursive = false)
        {
            if (!this.Exists)
                throw new ArgumentException("The given folder doesnt exist into the ftp server");
            if (String.IsNullOrWhiteSpace(localBasePath))
                throw new ArgumentException("The given folder doesnt exist into the ftp server");

            List<Downloader> l_Downloaders = new List<Downloader>();

            // Files at root
            foreach (FileInfo l_FtpFile in this.GetFiles().ToArray())
            {
                l_Downloaders.Add(
                    new DownloaderFile(
                        fileFTP: l_FtpFile,
                        fileHD: new System.IO.FileInfo(string.Format(@"{0}\{1}", localBasePath, l_FtpFile.Name))
                    )
                );
            }

            if (!recursive)
                return l_Downloaders;

            // Traverse files recursively
            Queue<DirectoryInfo> l_QueueTraverseFolders = new Queue<DirectoryInfo>();

            // Enqueu folders for the root
            foreach (DirectoryInfo child in this.GetFolders().ToArray())
                l_QueueTraverseFolders.Enqueue(child);

            while (0 != l_QueueTraverseFolders.Count)
            {
                DirectoryInfo l_FtpDir = l_QueueTraverseFolders.Dequeue();

                foreach (FileInfo l_FtpFile in l_FtpDir.GetFiles().ToArray())
                {
                    string l_AddedPath = l_FtpFile.Directory.FullPath.Remove(0, this.FullPath.Length);
                    string l_Folder_HD = string.Format(@"{0}\{1}\", localBasePath, l_AddedPath).ToValidPath_HD();
                    string l_File_HD = l_Folder_HD + l_FtpFile.Name;

                    l_Downloaders.Add(
                        new DownloaderFile(
                            fileFTP: l_FtpFile,
                            fileHD: new System.IO.FileInfo(l_File_HD)
                        )
                    );
                }

                // Enqueu folders for the next level
                foreach (DirectoryInfo child in l_FtpDir.GetFolders().ToArray())
                    l_QueueTraverseFolders.Enqueue(child);
            }

            return l_Downloaders;
        }
        
        public bool Delete()
        {
            return this.Delete(recursive: true);
        }
        private bool Delete(bool recursive)                    
        {
            if (!recursive) // Base case
            {
                // Check if existing
                if (!this.Exists)
                    return true;

                // Delete
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(this.Address);
                request.Credentials = new NetworkCredential(userName: this.Server.UserName, password: this.Server.Password);

                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return (FtpStatusCode.FileActionOK == response.StatusCode);
                }
            }
            else
            {
                Stack<DirectoryInfo> l_StackDeep = new Stack<DirectoryInfo>();
                this.StackDirectories(l_StackDeep, this);

                while (0 != l_StackDeep.Count)
                {
                    DirectoryInfo l_CurrentDir = l_StackDeep.Pop();
                    foreach (FileInfo l_File in l_CurrentDir.GetFiles().ToArray())
                        l_File.Delete();

                    l_CurrentDir.Delete(recursive: false);
                }

                return true;
            }
        }

        private void StackDirectories(Stack<DirectoryInfo> p_StackDeep, DirectoryInfo p_Current)
        {
            p_StackDeep.Push(p_Current);

            foreach (DirectoryInfo l_Child in p_Current.GetFolders().ToArray())
                this.StackDirectories(p_StackDeep, l_Child);

            return;
        }
        
        public override bool Equals(object obj)
        {
            DirectoryInfo objCasted = obj as DirectoryInfo;
            if (null == objCasted)
                return false;

            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        public override int GetHashCode()
        {
            return this.FullPath.GetHashCode();
        }
    }
}
