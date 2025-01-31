using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Gabi.Base
{
    /// <summary>
    ///     Gère une liste d'avertissements et fournit des fonctionnalités pour les ajouter, les afficher
    ///     et vérifier leur présence.
    /// </summary>
    /// <remarks>
    ///     Cette classe est utile pour collecter et gérer des messages d'avertissement tout au long
    ///     d'un processus, en évitant les doublons et en centralisant leur gestion.
    /// </remarks>
    public class WarningBuffer
    {
        private readonly List<string> _warningList = new();

        /// <summary>
        ///     Ajoute un message d'avertissement à la liste s'il n'est pas vide et s'il n'est pas déjà présent.
        /// </summary>
        /// <param name="warningMessage">Le message d'avertissement à ajouter.</param>
        /// <remarks>
        ///     Les messages d'avertissement en double ou vides ne sont pas ajoutés à la liste.
        /// </remarks>
        public void Add(string warningMessage)
        {
            if (!string.IsNullOrEmpty(warningMessage) && !_warningList.Contains(warningMessage))
                _warningList.Add(warningMessage);
        }

        /// <summary>
        ///     Affiche tous les messages d'avertissement dans la console en tant que journaux de niveau "Warning"
        ///     puis efface la liste.
        /// </summary>
        /// <remarks>
        ///     Cette méthode utilise Serilog pour enregistrer les avertissements, puis réinitialise la liste
        ///     pour éviter une duplication future.
        /// </remarks>
        public string Print()
        {
            var sb = new StringBuilder();
            foreach (var warning in _warningList)
                sb.AppendLine(warning);

            var warningStr = sb.ToString();
            Log.Warning(warningStr);
            _warningList.Clear();
            return warningStr;
        }

        /// <summary>
        ///     Indique s'il existe des avertissements non traités dans la liste.
        /// </summary>
        /// <returns><c>true</c> si des avertissements sont présents. Sinon, <c>false</c>.</returns>
        public bool HasWarning()
        {
            return _warningList.Count > 0;
        }
    }
}