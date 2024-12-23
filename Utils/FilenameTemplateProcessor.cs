using System;
using System.IO;
using System.Reflection;

namespace Gabi.Base.Utils
{
    public class FilenameTemplateProcessor
    {
        public static string Replace(string filename, string conf = null)
        {
            if (string.IsNullOrEmpty(conf)) conf = GetAssemblyName();
            var date = DateTime.Now;

            if (string.IsNullOrWhiteSpace(filename)) filename = "logs/$DATE$-$CONF$.log";
            filename = filename.Replace("$CONF$", conf);
            filename = filename.Replace("$TIME$", $"{date:HHmm}");
            filename = filename.Replace("$DATE$", $"{date:yyyyMMdd}");

            return filename;
        }

        private static string GetAssemblyName()
        {
            var appPath = Assembly.GetEntryAssembly()?.Location;
            return Path.GetFileNameWithoutExtension(appPath);
        }
    }
}