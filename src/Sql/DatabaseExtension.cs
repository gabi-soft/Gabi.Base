using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Serilog;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Classe interne contenant des extensions pour les opérations liées à la base de données.
    /// </summary>
    public static class DatabaseExtension
    {
        /// <summary>
        ///     Récupère la liste des colonnes d'une table depuis une base de données.
        /// </summary>
        /// <param name="connection">La connexion à la base de données.</param>
        /// <param name="tableName">Le nom de la table.</param>
        /// <returns>La liste des colonnes de la table.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        public static IEnumerable<DatabaseColumn> GetColumnsFromDatabase(this IDbConnection connection,
            string tableName)
        {
            const string query =
                @"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH 
                  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";

            var result = connection.Query(query, new { TableName = tableName });

            const string keysQuery =
                @"SELECT COLUMN_NAME
                  FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = @TableName";
#if NETCOREAPP1_0_OR_GREATER
            var keys = connection.Query<string>(keysQuery, new { TableName = tableName }).ToHashSet();
#else
            var keys = connection.Query<string>(keysQuery, new { TableName = tableName }).ToList();
#endif
            foreach (var row in result)
            {
                var isPk = keys.Contains(row.COLUMN_NAME + "");
                yield return new DatabaseColumn
                {
                    Name = row.COLUMN_NAME,
                    Type = SqlType.ConvertToSqlServerType(row.DATA_TYPE),
                    IsNullable = row.IS_NULLABLE.ToString().Equals("YES", StringComparison.OrdinalIgnoreCase),
                    IsPrimaryKey = isPk,
                    MaxLength = int.TryParse(row.CHARACTER_MAXIMUM_LENGTH + "", out int length) ? length : 0
                };
            }
        }

        /// <summary>
        ///     Récupère une instance de <see cref="DatabaseTable" /> à partir d'une base de données.
        /// </summary>
        /// <param name="connection">La connexion à la base de données.</param>
        /// <param name="tableName">Le nom de la table.</param>
        /// <returns>Une instance de <see cref="DatabaseTable" /> représentant la table.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        public static DatabaseTable GetTableFromDatabase(this IDbConnection connection, string tableName)
        {
            return new DatabaseTable(tableName, GetColumnsFromDatabase(connection, tableName).ToList());
        }

        /// <summary>
        ///     Supprime une table de la base de données de manière asynchrone.
        /// </summary>
        /// <param name="dbConnection">La connexion à la base de données.</param>
        /// <param name="table">La table à supprimer.</param>
        /// <returns>Une tâche représentant l'état de l'opération.</returns>
        public static async Task<bool> DropTableAsync(this IDbConnection dbConnection, DatabaseTable table)
        {
            if (!SqlObjectName.IsValidSqlObjectName(table.Name.AsSpan())) return false;

            var query = $"DROP TABLE [{table.Name}]";
            try
            {
                await dbConnection.ExecuteAsync(query);
            }
            catch (DbException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Crée une table dans la base de données de manière asynchrone.
        /// </summary>
        /// <param name="dbConnection">La connexion à la base de données.</param>
        /// <param name="table">La table a créé.</param>
        /// <returns>Une tâche représentant l'état de l'opération.</returns>
        /// <exception cref="ArgumentException">Erreur lors de la création</exception>
        public static async Task<bool> CreateTableAsync(this IDbConnection dbConnection, DatabaseTable table)
        {
            var columns = table.Columns;
            if (columns == null || columns.Count == 0)
                throw new ArgumentException("La table doit avoir au moins une colonne pour créer une table.");
            if (!SqlObjectName.IsValidSqlObjectName(table.Name.AsSpan()))
                throw new ArgumentException($"Le nom de la table ({table.Name}) n'est pas valide.");

            var primaryColomns = new List<string>();
            foreach (var column in columns)
                if (column.IsPrimaryKey)
                    primaryColomns.Add(
                        $"[{column.Name}]"); // Note : IsValidSqlObjectName was call in GetColumnDefinition

            var columnsString = string.Join(", ", columns.Select(static c => c.GetColumnDefinition()));
            if (primaryColomns.Count > 0)
                columnsString = string.Join(", ", columnsString, $"PRIMARY KEY ({string.Join(", ", primaryColomns)})");
            var query = $"CREATE TABLE [{table.Name}] ({columnsString})";

            var rowsAffected = await dbConnection.ExecuteAsync(query);
            return rowsAffected > 0;
        }

        /// <summary>
        ///     Vérifie si une table existe et la crée si elle n'existe pas.
        /// </summary>
        /// <param name="dbConnection">La connexion à la base de données.</param>
        /// <param name="table">La table à vérifier et éventuellement créer.</param>
        /// <returns>Une tâche représentant l'état de l'opération.</returns>
        /// <exception cref="ArgumentException">La table doit avoir au moins une colonne pour créer une table.</exception>
        public static async Task<bool> CreateTableIfNotExistsAsync(this IDbConnection dbConnection, DatabaseTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            if (string.IsNullOrWhiteSpace(table.Name))
                throw new ArgumentException("Le nom de la table ne peut pas être vide.", nameof(table));

            if (await TableExistAsync(dbConnection, table.Name))
                // La table existe déjà.
                return false;

            // La table n'existe pas, on la crée.
            return await dbConnection.CreateTableAsync(table);
        }

        /// <summary>
        ///     Vérifie si une table existe dans la base de données associée à la connexion SQL spécifiée.
        /// </summary>
        /// <param name="dbConnection">
        ///     L'instance de <see cref="IDbConnection" /> connectée à la base de données.
        /// </param>
        /// <param name="tableName">
        ///     Le nom de la table à vérifier dans la base de données.
        /// </param>
        /// <returns>
        ///     Une tâche asynchrone qui retourne un booléen indiquant si la table existe.
        /// </returns>
        public static async Task<bool> TableExistAsync(this IDbConnection dbConnection, string tableName)
        {
            const string tableExistsQuery = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = @TableName
                ) THEN 1 ELSE 0 END
            ";

            var tableExists = await dbConnection.QuerySingleAsync<int>(
                tableExistsQuery,
                new { TableName = tableName }
            );

            return tableExists == 1;
        }

        /// <summary>
        ///     Vérifie si la base de données associée à la connexion SQL spécifiée existe.
        /// </summary>
        /// <param name="dbConnection">
        ///     L'instance de <see cref="IDbConnection" /> connectée au serveur SQL.
        /// </param>
        /// <returns>
        ///     Une tâche asynchrone qui retourne un booléen indiquant si la base de données existe.
        /// </returns>
        public static async Task<bool> DatabaseExistAsync(this IDbConnection dbConnection)
        {
            const string query = "SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName";
            var res = await dbConnection.QueryFirstAsync<int>(query, new { databaseName = dbConnection.Database });
            return res > 0;
        }

        /// <summary>
        ///     Compresse la base de données associée à la connexion SQL spécifiée.
        /// </summary>
        /// <param name="dbConnection">
        ///     L'instance de <see cref="IDbConnection" /> connectée à la base de données à compresser.
        /// </param>
        /// <returns>
        ///     Une tâche représentant l'opération asynchrone.
        /// </returns>
        public static async Task CompressAsync(this IDbConnection dbConnection)
        {
            var database = dbConnection.Database;
            Log.Information("Compression de la base {Database}", database);
            var databaseExist = await DatabaseExistAsync(dbConnection);
            if (!databaseExist)
                return;

            const string compressSql = @"
                ALTER DATABASE [DATABASE_NAME]
                SET RECOVERY SIMPLE WITH ROLLBACK IMMEDIATE;
                USE [DATABASE_NAME];
                DBCC SHRINKDATABASE (0, 1, NOTRUNCATE);";
            await dbConnection.ExecuteAsync(compressSql.Replace("DATABASE_NAME", database));
        }
    }
}