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
        ///     Crée un logger avec config personnalisée ou par défaut.
        /// </summary>
        /// <param name="path">Chemin du fichier log. Vide → console uniquement.</param>
        /// <param name="loggerConfig">Config Serilog personnalisée. Null → config par défaut.</param>
        public static void CreateLogger(string path = "", LoggerConfiguration loggerConfig = null)
        {
            if (loggerConfig == null)
            {
                loggerConfig = new LoggerConfiguration();
                loggerConfig.WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            if (!string.IsNullOrEmpty(path))
            {
                var getLogPath = GetLogPath(path);
                try
                {
                    if (File.Exists(getLogPath))
                        File.Delete(getLogPath);
                }
                catch (SystemException e)
                {
                    Console.WriteLine($"Suppression du fichier de log {getLogPath} impossible : {e.Message}");
                }

                loggerConfig.WriteTo.Async(a => a.File(getLogPath));
            }

            try
            {
                Log.Logger = loggerConfig.CreateLogger();
            }
            catch (SystemException e)
            {
                Console.WriteLine($"Création du log impossible : {e.Message}");
            }
        }

        /// <summary>
        ///     Retourne le chemin du fichier log après traitement du nom.
        /// </summary>
        /// <param name="path">Chemin source avec nom de fichier. Null ou vide → null.</param>
        /// <returns>Chemin complet traité ou null.</returns>
        /// <exception cref="InvalidOperationException">Si le dossier est invalide.</exception>
        public static string GetLogPath(string path = null)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var logFolder = Path.GetDirectoryName(path);
            var logFilename = Path.GetFileName(path);
            logFilename = FilenameTemplateProcessor.Replace(logFilename);
            return Path.Combine(logFolder ?? throw new InvalidOperationException(), logFilename);
        }
    }
}