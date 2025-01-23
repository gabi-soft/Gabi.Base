using System;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Représente un objet SQL (table ou colonne) et fournit des méthodes pour valider son nom.
    ///     Pour pouvoir l'utliser dans des chaines de types $"[{Nom}]"
    /// </summary>
    public static class SqlObjectName
    {
        // Liste des caractères invalides pour les noms SQL, selon les conventions de SQL Server.
        private static readonly char[] InvalidChars = { '[', ']' };

        /// <summary>
        ///     Valide un nom d'objet SQL en utilisant une chaîne de caractères classique.
        /// </summary>
        /// <param name="name">Le nom a validé sous forme de chaîne de caractères.</param>
        /// <returns><c>true</c> si le nom est valide, sinon <c>false</c>.</returns>
        public static bool IsValidSqlObjectName(ReadOnlySpan<char> name)
        {
            // Vérifie si le nom est vide ou contient uniquement des espaces.
            if (name.Trim().Length == 0)
                return false;

            // Vérifie si le nom contient des caractères invalides comme '[' ou ']'.
            foreach (var invalidChar in InvalidChars)
                if (name.IndexOf(invalidChar) != -1)
                    return false;
            return true;
        }

        /// <summary>
        ///     Valide un nom d'objet SQL en utilisant une chaîne de caractères classique.
        /// </summary>
        /// <param name="name">Le nom a validé sous forme de chaîne de caractères.</param>
        /// <returns><c>true</c> si le nom est valide, sinon <c>false</c>.</returns>
        public static bool IsValidSqlObjectName(string name)
        {
            return IsValidSqlObjectName(name.AsSpan());
        }
    }
}