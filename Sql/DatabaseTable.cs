using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Représente une table de base de données avec ses colonnes.
    /// </summary>
    public class DatabaseTable
    {
        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="DatabaseTable" /> avec une liste de colonnes vide.
        /// </summary>
        public DatabaseTable()
        {
            Columns = new List<DatabaseColumn>();
        }

        /// <summary>
        ///     Initialise une nouvelle instance de la classe
        ///     <see cref="DatabaseTable" />
        ///     avec un nom et une liste de colonnes spécifiés.
        /// </summary>
        /// <param name="name">Le nom de la table de base de données.</param>
        /// <param name="columns">La liste des colonnes de la table.</param>
        public DatabaseTable(string name, List<DatabaseColumn> columns)
        {
            Name = name;
            Columns = columns ?? throw new ArgumentNullException(nameof(columns));
        }

        /// <summary>
        ///     Obtient ou définit le nom de la table de base de données.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Obtient ou définit la liste des colonnes de la table.
        /// </summary>
        public List<DatabaseColumn> Columns { get; set; }

        /// <summary>
        ///     Crée et retourne le type C# dynamique correspondant à la structure de la table.
        /// </summary>
        /// <returns>Le type C# dynamique représentant la table.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        public Type CreateType()
        {
            var props = new List<DynamicProperty>();

            foreach (var column in Columns)
            {
                var colType = SqlType.GetCSharpType(column.Type, column.IsNullable);
                var dynamicProperty = new DynamicProperty(column.Name, colType);
                props.Add(dynamicProperty);
            }

            return DynamicClassFactory.CreateType(props);
        }
    }
}