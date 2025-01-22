using System;
using System.IO;
using Gabi.Base.Utils;
using Serilog;

namespace Gabi.Base
{
    /// <summary>
    ///     Classe utilitaire pour la configuration du journal (logging).
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///     Crée un objet logger basé sur la configuration spécifiée.
        /// </summary>
        /// <param name="path">Le chemin complet du fichier journal (log).</param>
        public static void CreateLogger(string path)
        {
            var logFolder = Path.GetDirectoryName(path);
            var logFilename = Path.GetFileName(path);
            logFilename = FilenameTemplateProcessor.Replace(logFilename);
            var logFilepath = Path.Combine(logFolder ?? throw new InvalidOperationException(), logFilename);

            try
            {
                if (File.Exists(logFilepath))
                    File.Delete(logFilepath);
            }
            catch (SystemException e)
            {
                Console.WriteLine($"Suppression du fichier de log {logFilepath} impossible : {e.Message}");
            }

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Async(a => a.File(logFilepath))
                    .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (SystemException e)
            {
                Console.WriteLine($"Création du fichier de log {logFilepath} impossible : {e.Message}");
            }
        }
    }
}