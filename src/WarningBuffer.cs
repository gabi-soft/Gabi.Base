using System;
using Gabi.Base.Utils;
using Serilog.Events;

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
    public class WarningBuffer : LogBuffer
    {
        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="WarningBuffer"/> avec un niveau de log de type <see cref="LogEventLevel.Warning"/>.
        /// </summary>
        public WarningBuffer() : base(LogEventLevel.Warning)
        {
        }

        /// <summary>
        ///     Indique s'il existe des avertissements non traités dans la liste.
        /// </summary>
        /// <returns><c>true</c> si des avertissements sont présents. Sinon, <c>false</c>.</returns>
        /// <remarks>
        ///     Cette méthode est maintenant obsolète. Utilisez <see cref="LogBuffer.HasLogs"/> pour vérifier si des logs (y compris des avertissements) sont présents.
        /// </remarks>
        [Obsolete("Cette méthode est obsolète. Appelez HasLogs() à la place.")]
        public bool HasWarning()
        {
            return HasLogs();
        }
    }
}