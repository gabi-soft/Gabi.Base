using System;

namespace Gabi.Base
{
    /// <summary>
    ///     Exception personnalisée pour les erreurs spécifiques à la lib
    /// </summary>
    public class BaseException : Exception
    {
        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="BaseException" /> avec un message spécifié.
        /// </summary>
        /// <param name="message">Le message d'erreur qui explique la raison de l'exception.</param>
        public BaseException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="BaseException" /> avec un message d'erreur spécifié
        ///     et une référence à l'exception interne qui est la cause de cette exception.
        /// </summary>
        /// <param name="message">Le message d'erreur qui explique la raison de l'exception.</param>
        /// <param name="innerException">L'exception interne qui est la cause de cette exception.</param>
        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}