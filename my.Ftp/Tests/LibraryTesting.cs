using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;        
using IO = System.IO;   //Shortcut for typing. It doesnt include the extensors

namespace my.Ftp.Tests
{
    [TestClass]
    public class LibraryTesting
    {
        [TestInitialize]
        public void Initialize()
        {
            this.Cleanup();

            IO.Directory.CreateDirectory("C:\\temp\\");

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(
                (new IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).Parent.Parent.FullName + "\\Tests\\HDFtp",
                "C:\\temp\\HDFtp"
            );

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(
                (new IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).Parent.Parent.FullName + "\\Tests\\HDLocal",
                "C:\\temp\\HDLocal"
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var dirPath in IO.Directory.GetDirectories("C:\\temp\\", "*", IO.SearchOption.AllDirectories))
            {
                try { IO.Directory.Delete(dirPath); }
                catch (Exception) { }
            }
            foreach (var filePath in IO.Directory.GetFiles("C:\\temp\\", "*.*", IO.SearchOption.AllDirectories))
            {
                try { IO.File.Delete(filePath); }
                catch (Exception) { }
            }
        }

        #region Directories

        [TestMethod]
        public void Directory_Exists_yes_root()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp.Exists);
        }

        [TestMethod]
        public void Directory_Exists_yes_nested()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp_Folder_1.Exists);
        }

        [TestMethod]
        public void Directory_Exists_no_root()
        {
            Assert.IsFalse(Constants.Ftp.Folders.NotExists_root.Exists);
        }

        [TestMethod]
        public void Directory_Exists_no_nested()
        {
            Assert.IsFalse(Constants.Ftp.Folders.NotExists_nested.Exists);
        }

        [TestMethod]
        public void Directory_Create_existing_root()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp.Create());
        }

        [TestMethod]
        public void Directory_Create_existing_nested()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp_Folder_1.Create());
        }

        [TestMethod]
        public void Directory_Create_non_existing_root()
        {
            Assert.IsTrue(Constants.Ftp.Folders.New_root.Create());
        }

        [TestMethod]
        public void Directory_Create_non_existing_nested()
        {
            Assert.IsTrue(Constants.Ftp.Folders.New_nested.Create());
        }

        [TestMethod]
        public void Directory_Delete_root_exists_yes()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp.Delete());
        }

        [TestMethod]
        public void Directory_Delete_root_exists_no()
        {
            Assert.IsTrue(Constants.Ftp.Folders.NotExists_root.Delete());
        }

        [TestMethod]
        public void Directory_Delete_nested_exists_yes()
        {
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp_Folder_1.Delete());
        }

        [TestMethod]
        public void Directory_Delete_nested_exists_no()
        {
            Assert.IsTrue(Constants.Ftp.Folders.NotExists_nested.Delete());
        }

        [TestMethod]
        public void Directory_GetFolderChild_exists_yes()
        {
            var f = Constants.Ftp.Folders.HDFtp.GetFolderChild(Constants.Ftp.Folders.HDFtp_Folder_1.Name);

            Assert.IsNotNull(f);
            Assert.IsTrue(f.Exists);
            Assert.IsTrue(String.Equals(f.Name, Constants.Ftp.Folders.HDFtp_Folder_1.Name));
        }

        [TestMethod]
        public void Directory_GetFolderChild_exists_no()
        {
            var f = Constants.Ftp.Folders.HDFtp.GetFolderChild(Constants.Ftp.Folders.NotExists_nested.Name);

            Assert.IsNotNull(f);
            Assert.IsFalse(f.Exists);
            Assert.IsTrue(String.Equals(f.Name, Constants.Ftp.Folders.NotExists_nested.Name));
        }

        [TestMethod]
        public void Directory_GetFolders_no_filter()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders();

            Assert.IsNotNull(fs);
            Assert.IsTrue(5 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFolders_filter_no_results()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders("dummy_filter");

            Assert.IsNotNull(fs);
            Assert.IsTrue(0 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFolders_filter_exact()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders(Constants.Ftp.Folders.HDFtp_Folder_1.Name, SearchOption.Exact);

            Assert.IsNotNull(fs);
            Assert.IsTrue(1 == fs.Count());
            Assert.IsTrue(Constants.Ftp.Folders.HDFtp_Folder_1.Name == fs.First().Name);
        }

        [TestMethod]
        public void Directory_GetFolders_filter_startswith()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders("Folder", SearchOption.StartsWith);

            Assert.IsNotNull(fs);
            Assert.IsTrue(3 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFolders_filter_contains()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders("ther_", SearchOption.Contains);

            Assert.IsNotNull(fs);
            Assert.IsTrue(2 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFolders_filter_endswith()
        {
            var fs = Constants.Ftp.Folders.HDFtp.GetFolders("Folder", SearchOption.EndsWith);

            Assert.IsNotNull(fs);
            Assert.IsTrue(2 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFiles_no_filter()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles();

            Assert.IsNotNull(fs);
            Assert.IsTrue(6 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFiles_filter_no_results()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles("dummy_filter");

            Assert.IsNotNull(fs);
            Assert.IsTrue(0 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFiles_filter_exact()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles(Constants.Ftp.Files.Exact_name.Name, SearchOption.Exact);

            Assert.IsNotNull(fs);
            Assert.IsTrue(1 == fs.Count());
            Assert.IsTrue(Constants.Ftp.Files.Exact_name.Name == fs.First().Name);
        }

        [TestMethod]
        public void Directory_GetFiles_filter_startswith()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles("File_", SearchOption.StartsWith);

            Assert.IsNotNull(fs);
            Assert.IsTrue(3 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFiles_filter_contains()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles("ther_", SearchOption.Contains);

            Assert.IsNotNull(fs);
            Assert.IsTrue(2 == fs.Count());
        }

        [TestMethod]
        public void Directory_GetFiles_filter_endswith()
        {
            var fs = Constants.Ftp.Folders.HDFtp_Folder_1.GetFiles("file", SearchOption.EndsWith);

            Assert.IsNotNull(fs);
            Assert.IsTrue(2 == fs.Count());
        }

        #endregion

        #region Files

        [TestMethod]
        public void File_exists_yes()
        {
            Assert.IsTrue(Constants.Ftp.Files.File_1_1.Exists);
        }

        [TestMethod]
        public void File_exists_no()
        {
            Assert.IsFalse(Constants.Ftp.Files.NotExists.Exists);
        }

        [TestMethod]
        public void File_getSize()
        {
            Assert.IsTrue(56 == Constants.Ftp.Files.File_1_1.Size);
        }
        
        [TestMethod]
        public void File_delete_exists_yes()
        {
            Assert.IsTrue(Constants.Ftp.Files.File_1_1.Delete());
        }

        [TestMethod]
        public void File_delete_exists_no()
        {
            Assert.IsFalse(Constants.Ftp.Files.NotExists.Delete());
        }

        [TestMethod]
        public void File_renameTo_exists_yes()
        {
            Ftp.FileInfo ftpFile = new Ftp.FileInfo(Constants.Ftp.Files.File_1_1.Directory, Constants.Ftp.Files.File_1_1.Name);
            ftpFile.RenameTo("File_1_1.renamed");

            Assert.AreSame("File_1_1.renamed", ftpFile.Name);
            Assert.IsTrue(string.Equals("File_1_1.renamed", Constants.Ftp.Files.File_1_1.Directory.GetFiles("File_1_1.renamed", SearchOption.Exact).First().Name));
        }

        [TestMethod]
        public void File_renameTo_exists_no()
        {
            Assert.IsFalse(Constants.Ftp.Files.NotExists.RenameTo("File_1_1.renamed"));
        }

        #endregion

        #region Download

        [TestMethod]
        public void DownFileSync_file_exists_yes()
        {
            IO.FileInfo downloadedFile = new IO.FileInfo("C:\\temp\\Download\\" + Constants.Ftp.Files.File_1_1.Name);
            DownloaderFile df = new DownloaderFile(Constants.Ftp.Files.File_1_1, downloadedFile);
            df.StartDownload(async: false);
            
            downloadedFile.Refresh();
            byte[] file_ftp = IO.File.ReadAllBytes(Constants.HDFtp.Files.File_1_1.FullName);
            byte[] file_hd  = IO.File.ReadAllBytes(downloadedFile.FullName);

            Assert.IsTrue(downloadedFile.Exists);
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(file_ftp, file_hd));
        }

        [TestMethod]
        public void DownFileSync_file_exists_no()
        {
            IO.FileInfo downloadedFile = new IO.FileInfo("C:\\temp\\download\\" + Constants.Ftp.Files.NotExists.Name);
            DownloaderFile df = new DownloaderFile(Constants.Ftp.Files.NotExists, downloadedFile);
            try
            {
                df.StartDownload(async: false);
                Assert.IsTrue(false);
            }
            catch(Exception e)
            {
                Assert.IsTrue(e is System.Net.WebException);
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void DownDataSync_file_exists_yes()
        {
            DownloaderData dd = new DownloaderData(Constants.Ftp.Files.File_1_1);
            dd.StartDownload(async: false);

            byte[] file_ftp = IO.File.ReadAllBytes(Constants.HDFtp.Files.File_1_1.FullName);

            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(file_ftp, dd.DownloadedData));
        }

        [TestMethod]
        public void DownDataSync_file_exists_no()
        {
            DownloaderData dd = new DownloaderData(Constants.Ftp.Files.NotExists);

            try
            {
                dd.StartDownload(async: false);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is System.Net.WebException);
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region Upload

        [TestMethod]
        public void UpFileSync_file_exists_yes()
        {
            Ftp.DirectoryInfo ftpDir = new Ftp.DirectoryInfo(Constants.Ftp.FTP_TestServer, "HDFtp/Upload");
            Ftp.FileInfo ftpFile = new Ftp.FileInfo(ftpDir, "UploadTestFile");
            UploaderFile uf = new UploaderFile(ftpFile, Constants.HD.Files.File_1_1);
            uf.StartUpload(async: false);

            byte[] file_ftp = IO.File.ReadAllBytes("C:\\temp\\HDFtp\\Upload\\UploadTestFile");
            byte[] file_hd = IO.File.ReadAllBytes(Constants.HD.Files.File_1_1.FullName);

            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(file_ftp, file_hd));
        }

        [TestMethod]
        public void UpFileSync_file_exists_no()
        {
            Ftp.DirectoryInfo ftpDir = new Ftp.DirectoryInfo(Constants.Ftp.FTP_TestServer, "HDFtp/Upload");
            Ftp.FileInfo ftpFile = new Ftp.FileInfo(ftpDir, "UploadTestFile");

            try
            {
                UploaderFile uf = new UploaderFile(ftpFile, Constants.HD.Files.NotExists);
                uf.StartUpload(async: false);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is IO.FileNotFoundException);
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void UpDataSync_file_exists_yes()
        {
            Ftp.DirectoryInfo ftpDir = new Ftp.DirectoryInfo(Constants.Ftp.FTP_TestServer, "HDFtp/Upload");
            Ftp.FileInfo ftpFile = new Ftp.FileInfo(ftpDir, "UploadTestFile");
            byte[] file_hd = IO.File.ReadAllBytes(Constants.HD.Files.File_1_1.FullName);
            UploaderData ud = new UploaderData(ftpFile, file_hd);
            ud.StartUpload(async: false);

            byte[] file_ftp = IO.File.ReadAllBytes("C:\\temp\\HDFtp\\Upload\\UploadTestFile");
            
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(file_ftp, file_hd));
        }

        [TestMethod]
        public void UpDataSync_file_exists_no()
        {
            Ftp.DirectoryInfo ftpDir = new Ftp.DirectoryInfo(Constants.Ftp.FTP_TestServer, "HDFtp/Upload");
            Ftp.FileInfo ftpFile = new Ftp.FileInfo(ftpDir, "UploadTestFile");

            try
            {
                byte[] file_hd = IO.File.ReadAllBytes(Constants.HD.Files.NotExists.FullName);
                UploaderData ud = new UploaderData(ftpFile, file_hd);
                ud.StartUpload(async: false);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is IO.FileNotFoundException);
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region Create Downloaders / Uploaders

        [TestMethod]
        public void Directory_CreateDownloaders_files_no()
        {
            var downs = Constants.Ftp.Folders.HDFtp.CreateFileDownloaders("C:\\temp\\download", recursive: false);

            Assert.IsNotNull(downs);
            Assert.IsTrue(0 == downs.Count());
        }

        [TestMethod]
        public void Directory_CreateDownloaders_files_no_rec_yes()
        {
            var downs = Constants.Ftp.Folders.HDFtp_Folder_3.CreateFileDownloaders("C:\\temp\\download", recursive: true);

            Assert.IsNotNull(downs);
            Assert.IsTrue(0 == downs.Count());
        }

        [TestMethod]
        public void Directory_CreateDownloaders_files_yes()
        {
            var downs = Constants.Ftp.Folders.HDFtp_Folder_1.CreateFileDownloaders("C:\\temp\\download", recursive: false);

            Assert.IsNotNull(downs);
            Assert.IsTrue(6 == downs.Count());
        }

        [TestMethod]
        public void Directory_CreateDownloaders_files_yes_rec_yes()
        {
            var downs = Constants.Ftp.Folders.HDFtp.CreateFileDownloaders("C:\\temp\\download", recursive: true);

            Assert.IsNotNull(downs);
            Assert.IsTrue(8 == downs.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_no()
        {
            var ups = Constants.HD.Folders.HDLocal_Folder_2.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*.*",
                recursive: false);

            Assert.IsNotNull(ups);
            Assert.IsTrue(0 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_no_rec_yes()
        {
            var ups = Constants.HD.Folders.HDLocal_Folder_2.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*.*",
                recursive: true);

            Assert.IsNotNull(ups);
            Assert.IsTrue(0 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_yes_rec_no()
        {
            var ups = Constants.HD.Folders.HDLocal.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*.*",
                recursive: false);

            Assert.IsNotNull(ups);
            Assert.IsTrue(1 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_yes_rec_yes()
        {
            var ups = Constants.HD.Folders.HDLocal.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*.*",
                recursive: true);

            Assert.IsNotNull(ups);
            Assert.IsTrue(3 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_yes_rec_yes_filter_1()
        {
            var ups = Constants.HD.Folders.HDLocal.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*_1_*.*",
                recursive: true);

            Assert.IsNotNull(ups);
            Assert.IsTrue(2 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_yes_rec_yes_filter_2()
        {
            var ups = Constants.HD.Folders.HDLocal.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*File*.*",
                recursive: true);

            Assert.IsNotNull(ups);
            Assert.IsTrue(3 == ups.Count());
        }

        [TestMethod]
        public void Directory_CreateUploaders_files_yes_rec_yes_filter_3()
        {
            var ups = Constants.HD.Folders.HDLocal.CreateUploaders(
                Constants.Ftp.Folders.HDFtp,
                searchPattern: "*.txt",
                recursive: true);

            Assert.IsNotNull(ups);
            Assert.IsTrue(1 == ups.Count());
        }

        [TestMethod]
        public void File_CreateUploader_files_exists()
        {
            var ups = Constants.HD.Files.File_1_1.CreateUploader(Constants.Ftp.Folders.HDFtp);

            Assert.IsNotNull(ups);
        }

        [TestMethod]
        public void File_CreateUploader_files_exists_no()
        {
            var ups = Constants.HD.Files.NotExists.CreateUploader(Constants.Ftp.Folders.HDFtp);

            Assert.IsNull(ups);
        }

        #endregion
    }
}
