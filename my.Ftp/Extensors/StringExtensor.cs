using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class StringExtensor
    {
        public static string RemoveStart(this string str, string[] starts)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            foreach (string start in starts)
                if (!string.IsNullOrEmpty(str) && str.StartsWith(start))
                    str = str.Remove(0, start.Length);

            return str;
        }

        public static string Replace(this string str, Tuple<string, string>[] replacements)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            foreach (var replacement in replacements)
                if (!string.IsNullOrEmpty(str))
                    str = str.Replace(replacement.Item1, replacement.Item2);

            return str;
        }

        public static string ToValidPath_HD(this string str)
        {
            Tuple<string, string>[] replacements = new Tuple<string, string>[]
            {
                new Tuple<string, string>(@"/", @"\"), 
                new Tuple<string, string>(@"\\", @"\")
            };

            return str.Replace(replacements);
        }

        public static string ToValidPath_FTP(this string str)
        {
            Tuple<string, string>[] replacements = new Tuple<string, string>[]
            {
                new Tuple<string, string>(@"//", @"/"), 
                new Tuple<string, string>(@"\\", @"/"), 
                new Tuple<string, string>(@"\", @"/")
            };
            
            return str.Replace(replacements);
        }

    }
}

