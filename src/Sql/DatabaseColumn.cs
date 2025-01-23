using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Représente une colonne d'une table de base de données.
    /// </summary>
    public class DatabaseColumn
    {
        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="DatabaseColumn" />.
        /// </summary>
        public DatabaseColumn()
        {
        }

        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="DatabaseColumn" /> avec les propriétés spécifiées.
        /// </summary>
        /// <param name="name">Le nom de la colonne.</param>
        /// <param name="type">Le type de données de la colonne.</param>
        /// <param name="isNullable">Indique si la colonne autorise les valeurs nulles.</param>
        /// <param name="maxLength">La longueur maximale de la colonne (par défaut, 0).</param>
        /// <param name="isPrimaryKey">Indique si la colonne est une clé primaire (par défaut, false).</param>
        public DatabaseColumn(string name, SqlServerType type, bool isNullable, int maxLength = 0,
            bool isPrimaryKey = false)
        {
            Name = name;
            Type = type;
            IsNullable = isNullable;
            MaxLength = maxLength;
            IsPrimaryKey = isPrimaryKey;
        }

        /// <summary>
        ///     Obtient ou définit le nom de la colonne.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Obtient ou définit le type de données de la colonne.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public SqlServerType Type { get; set; }

        /// <summary>
        ///     Obtient ou définit une valeur indiquant si la colonne autorise les valeurs nulles.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        ///     Obtient ou définit une valeur indiquant si la colonne est une clé primaire.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        ///     Obtient ou définit la longueur maximale de la colonne.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        ///     Obtient ou définit la longueur maximale de la colonne.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        ///     Obtient la définition de la colonne sous forme de chaîne SQL.
        /// </summary>
        /// <returns>La définition de la colonne sous forme de chaîne SQL.</returns>
        public string GetColumnDefinition()
        {
            if (!SqlObjectName.IsValidSqlObjectName(Name.AsSpan()))
                throw new ArgumentException($"Le nom de la colonne ({Name}) n'est pas valide.");

            string columnDefinition;

            var maxLength = GetMaxLength();
            var precision = GetPrecision();

            if (Type is SqlServerType.NVarChar)
                columnDefinition = $"[{Name}] {SqlType.ConvertToSqlDataType(Type)}(max)";
            else if (maxLength <= 0)
                columnDefinition = $"[{Name}] {SqlType.ConvertToSqlDataType(Type)}";
            else if (precision <= 0)
                columnDefinition = $"[{Name}] {SqlType.ConvertToSqlDataType(Type)}({maxLength})";
            else
                columnDefinition = $"[{Name}] {SqlType.ConvertToSqlDataType(Type)}({maxLength}, {precision})";

            if (!IsNullable) columnDefinition += " NOT NULL";

            return columnDefinition;
        }

        public int GetMaxLength()
        {
            if (MaxLength > 0)
                return MaxLength;
            if (Type == SqlServerType.Decimal)
                return 38;
            return Precision;
        }

        public int GetPrecision()
        {
            if (Precision > 0)
                return Precision;
            if (Type == SqlServerType.Decimal)
                // Note : Dans le cas d'un décimal SQL Serveur peut stocker une précision de 17, mais C# n'a pas assez de bit pour
                // Stocker le chiffre MAX en décimal.
                return 15;
            return Precision;
        }
    }
}