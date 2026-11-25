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
            // Location est vide en single-file et l'assembly d'entrée peut être null (tests, hôtes natifs)
            var appPath = Assembly.GetEntryAssembly()?.Location;
            var name = string.IsNullOrEmpty(appPath)
                ? AppDomain.CurrentDomain.FriendlyName
                : Path.GetFileNameWithoutExtension(appPath);
            return string.IsNullOrEmpty(name) ? "app" : name;
        }
    }
}