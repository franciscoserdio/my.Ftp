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
    public static class File
    {
        public static bool Exists(DirectoryInfo directory, string fileName)
        {
            return (new FileInfo(directory, fileName)).Exists;
        }

        public static long? GetSize(DirectoryInfo directory, string fileName)
        {
            return (new FileInfo(directory, fileName)).Size;
        }

        public static bool RenameTo(DirectoryInfo directory, string fileOldName, string fileNewName)
        {
            return (new FileInfo(directory, fileOldName)).RenameTo(fileNewName);
        }

        public static bool Delete(DirectoryInfo directory, string fileName)
        {
            return (new FileInfo(directory, fileName)).Delete();
        }
    }
}
