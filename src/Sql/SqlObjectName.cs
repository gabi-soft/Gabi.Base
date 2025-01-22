using System;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Représente un objet SQL (table ou colonne) et fournit des méthodes pour valider son nom.
    ///     Pour pouvoir l'utliser dans des chaines de types $"[{Nom}]"
    /// </summary>
    public class SqlObjectName
    {
        // Liste des caractères invalides pour les noms SQL, selon les conventions de SQL Server.
        private static readonly char[] InvalidChars = new char[] { '[', ']' };

        /// <summary>
        ///     Valide un nom d'objet SQL en utilisant une chaîne de caractères classique.
        /// </summary>
        /// <param name="name">Le nom à valider sous forme de chaîne de caractères.</param>
        /// <returns>
        /// <returns><c>true</c> si le nom est valide, sinon <c>false</c>.</returns>
#if NET8_0
        public static bool IsValidSqlObjectName(ReadOnlySpan<char> name)
        {
            // Vérifie si le nom est vide ou contient uniquement des espaces.
            if (name.Trim().IsEmpty)
                return false;

            // Vérifie si le nom contient des caractères invalides comme '[' ou ']'.
            foreach (var invalidChar in InvalidChars)
            {
                if (name.IndexOf(invalidChar) != -1)
                    return false;
            }
            return true;
        }
#else
        public static bool IsValidSqlObjectName(string name)
        {
            // Vérifie si le nom est nul ou vide, ou contient uniquement des espaces.
            if (string.IsNullOrEmpty(name))
                return false;

            // Vérifie si le nom contient des caractères invalides comme '[' ou ']'.
            foreach (var invalidChar in InvalidChars)
            {
                if (name.IndexOf(invalidChar) != -1)
                    return false;
            }
            return true;
        }
#endif
    }
}