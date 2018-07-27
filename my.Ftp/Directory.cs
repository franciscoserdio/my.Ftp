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
    public static class Directory
    {
        public static bool Create(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).Create();
        }

        public static bool CreateWithSubfolders(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).CreateWithSubfolders();
        }

        public static bool Exists(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).Exists;
        }

        public static bool Delete(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).Delete();
        }

        public static IEnumerable<FileInfo> GetFiles(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).GetFiles();
        }

        public static IEnumerable<FileInfo> GetFiles(Server server, string directoryFullName, string searchPattern, SearchOption searchOption = SearchOption.Exact)
        {
            return (new DirectoryInfo(server, directoryFullName)).GetFiles(p_SearchPattern: searchPattern, p_SearchOption: searchOption);
        }

        public static IEnumerable<DirectoryInfo> GetFolders(Server server, string directoryFullName)
        {
            return (new DirectoryInfo(server, directoryFullName)).GetFolders();
        }

        public static IEnumerable<DirectoryInfo> GetFolders(Server server, string directoryFullName, string searchPattern, SearchOption searchOption = SearchOption.Exact)
        {
            return (new DirectoryInfo(server, directoryFullName)).GetFolders(p_SearchPattern: searchPattern, p_SearchOption: searchOption);
        }

        public static IEnumerable<Downloader> CreateFileDownloaders(Server server, string ftpDirectoryFullName, string localBasePath, bool recursive = false)
        {
            return (new DirectoryInfo(server, ftpDirectoryFullName)).CreateFileDownloaders(recursive: recursive, localBasePath: localBasePath);
        }
    }
}
