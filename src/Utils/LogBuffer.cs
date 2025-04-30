using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Events;

namespace Gabi.Base.Utils
{
    /// <summary>
    ///     Gère une liste d'événements de journalisation et fournit des fonctionnalités pour les ajouter, les afficher
    ///     et vérifier leur présence, avec un niveau de log spécifié.
    /// </summary>
    public class LogBuffer
    {
        private readonly LogEventLevel _level;
        private readonly List<string> _logList = new();

        public LogBuffer(LogEventLevel logLevel)
        {
            _level = logLevel;
        }

        /// <summary>
        ///     Ajoute un message de log à la liste s'il n'est pas vide et s'il n'est pas déjà présent.
        /// </summary>
        /// <param name="logMessage">Le message de log à ajouter.</param>
        public void Add(string logMessage)
        {
            if (!string.IsNullOrEmpty(logMessage) && !_logList.Contains(logMessage))
                _logList.Add(logMessage);
        }

        /// <summary>
        ///     Affiche tous les messages de log dans la console en utilisant le niveau de log spécifié
        ///     puis efface la liste.
        /// </summary>
        public string Print()
        {
            var sb = new StringBuilder();
            foreach (var log in _logList)
                sb.AppendLine(log);

            var logStr = sb.ToString();
            Log.Write(_level, logStr); // Log the message with the appropriate level
            _logList.Clear();
            return logStr;
        }

        /// <summary>
        ///     Indique s'il existe des logs non traités dans la liste.
        /// </summary>
        /// <returns><c>true</c> si des logs sont présents. Sinon, <c>false</c>.</returns>
        public bool HasLogs()
        {
            return _logList.Count > 0;
        }
    }
}