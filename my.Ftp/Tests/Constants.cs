using System;
using my.Ftp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;

namespace my.Ftp.Tests
{
    public static class Constants
    {
        internal static class Ftp
        {
            public static Server FTP_TestServer = new Server()
            {
                Name = "FTP_TestServer",
                Address = "ftp://127.0.0.1",
                Port = 21,
                UserName = "anonymous",
                Password = "anonymous"
            };

            internal static class Folders
            {
                public static DirectoryInfo HDFtp               = new DirectoryInfo(FTP_TestServer, @"\HDFtp");
                public static DirectoryInfo HDFtp_Folder_1      = new DirectoryInfo(FTP_TestServer, @"\HDFtp\Folder_1");
                public static DirectoryInfo HDFtp_Folder_2      = new DirectoryInfo(FTP_TestServer, @"\HDFtp\Folder_2");
                public static DirectoryInfo HDFtp_Folder_3      = new DirectoryInfo(FTP_TestServer, @"\HDFtp\Folder_3");
                public static DirectoryInfo New_root            = new DirectoryInfo(FTP_TestServer, @"\New_root");
                public static DirectoryInfo New_nested          = new DirectoryInfo(FTP_TestServer, @"\HDFtp\New_nested");
                public static DirectoryInfo NotExists_root      = new DirectoryInfo(FTP_TestServer, @"\NotExists");
                public static DirectoryInfo NotExists_nested    = new DirectoryInfo(FTP_TestServer, @"\HDFtp\NotExists_nested");
            }

            internal static class Files
            {
                public static FileInfo Another_file = new FileInfo(Folders.HDFtp_Folder_1, "Another_file");
                public static FileInfo Exact_name   = new FileInfo(Folders.HDFtp_Folder_1, "Exact_name");
                public static FileInfo File_1_1     = new FileInfo(Folders.HDFtp_Folder_1, "File_1_1.txt");
                public static FileInfo File_1_2     = new FileInfo(Folders.HDFtp_Folder_1, "File_1_2");
                public static FileInfo File_1_3     = new FileInfo(Folders.HDFtp_Folder_1, "File_1_3.txt");
                public static FileInfo Other_file   = new FileInfo(Folders.HDFtp_Folder_1, "Other_file");
                public static FileInfo NotExists    = new FileInfo(Folders.HDFtp_Folder_1, "NotExists");
            }
        }

        internal static class HD
        {
            internal static class Folders
            {
                public static IO.DirectoryInfo HDLocal          = new IO.DirectoryInfo("C:\\temp\\HDLocal\\");
                public static IO.DirectoryInfo HDLocal_Folder_1 = new IO.DirectoryInfo("C:\\temp\\HDLocal\\Folder_1\\");
                public static IO.DirectoryInfo HDLocal_Folder_2 = new IO.DirectoryInfo("C:\\temp\\HDLocal\\Folder_2\\");
            }

            internal static class Files
            {
                public static IO.FileInfo File_1    = new IO.FileInfo("C:\\temp\\HDLocal\\File_1");
                public static IO.FileInfo File_1_1  = new IO.FileInfo("C:\\temp\\HDLocal\\Folder_1\\File_1_1.txt");
                public static IO.FileInfo File_1_2  = new IO.FileInfo("C:\\temp\\HDLocal\\Folder_1\\File_1_2");
                public static IO.FileInfo NotExists = new IO.FileInfo("C:\\temp\\HDLocal\\NotExists");
            }
        }

        internal static class HDFtp
        {
            internal static class Files
            {
                public static IO.FileInfo File_1_1 = new IO.FileInfo("C:\\temp\\HDFtp\\Folder_1\\File_1_1.txt");
            }
        }
    }
}
