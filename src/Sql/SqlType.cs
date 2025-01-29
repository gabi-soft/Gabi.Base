using System;
using Serilog;

namespace Gabi.Base.Sql
{
    /// <summary>
    ///     Classe interne pour la conversion entre les types de données SQL Server et les types C#.
    /// </summary>
    internal static class SqlType
    {
        /// <summary>
        ///     Convertit une représentation de chaîne de type de données SQL Server en une énumération
        ///     <see cref="SqlServerType" />.
        /// </summary>
        /// <param name="dataType">La représentation de chaîne du type de données SQL Server.</param>
        /// <returns>Le type de données SQL Server converti.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        public static SqlServerType ConvertToSqlServerType(string dataType)
        {
            dataType = dataType.ToLower().Trim();
            foreach (SqlServerType type in Enum.GetValues(typeof(SqlServerType)))
                if (type.ToString().ToLower() == dataType)
                    return type;
            throw new BaseException($"Type de données non pris en charge: {dataType}");
        }

        /// <summary>
        ///     Convertit un type de données SQL Server en une représentation de chaîne du type de données SQL.
        /// </summary>
        /// <param name="sqlServerType">Le type de données SQL Server à convertir.</param>
        /// <returns>La représentation de chaîne du type de données SQL Server.</returns>
        public static string ConvertToSqlDataType(SqlServerType sqlServerType)
        {
            if (sqlServerType is SqlServerType.SmallInt or SqlServerType.TinyInt)
                Log.Warning(
                    "Pour avoir un uniformité des types de colonnes, il est recommandé de d'éviter d'utiliser {SqlServerType} au profit du type Int.",
                    sqlServerType.ToString());
            else if (sqlServerType is SqlServerType.NText)
                Log.Warning(
                    "Ntext est obsolète depuis SQL Server 2005, il est recommandé de l'éviter au profit de Nvarchar.");
            return sqlServerType.ToString().ToLower();
        }

        /// <summary>
        ///     Obtient le type C# correspondant à un type de données SQL Server spécifié.
        /// </summary>
        /// <param name="sqlServerType">Le type de données SQL Server.</param>
        /// <param name="isNullable">Indique si le champ doit être Nullable ou non</param>
        /// <returns>Le type C# correspondant.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        public static Type GetCSharpType(SqlServerType sqlServerType, bool isNullable = false)
        {
            return isNullable ? GetNullableCSharpType(sqlServerType) : GetNotNullCSharpType(sqlServerType);
        }

        /// <summary>
        ///     Obtient le type C# correspondant à un type non null de données SQL Server spécifié.
        /// </summary>
        /// <param name="sqlServerType">Le type de données SQL Server.</param>
        /// <returns>Le type C# correspondant.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        private static Type GetNotNullCSharpType(SqlServerType sqlServerType)
        {
            switch (sqlServerType)
            {
                case SqlServerType.BigInt:
                    return typeof(long);
                case SqlServerType.Bit:
                    return typeof(bool);
                case SqlServerType.Date:
                    return typeof(DateTime);
                case SqlServerType.DateTime:
                    return typeof(DateTime);
                case SqlServerType.Decimal:
                    return typeof(decimal);
                case SqlServerType.Float:
                    return typeof(double);
                case SqlServerType.Int:
                    return typeof(int);
                case SqlServerType.SmallInt:
                    return typeof(short);
                case SqlServerType.TinyInt:
                    return typeof(byte);
                case SqlServerType.VarBinary:
                    return typeof(byte[]);
                case SqlServerType.NText:
                case SqlServerType.NVarChar:
                case SqlServerType.VarChar:
                    return typeof(string);
                default:
                    throw new BaseException($"Type de données non pris en charge: {sqlServerType}");
            }
        }

        /// <summary>
        ///     Obtient le type C# correspondant à un type Nullable de données SQL Server spécifié.
        /// </summary>
        /// <param name="sqlServerType">Le type de données SQL Server.</param>
        /// <returns>Le type C# correspondant.</returns>
        /// <exception cref="BaseException">Type de données non pris en charge.</exception>
        private static Type GetNullableCSharpType(SqlServerType sqlServerType)
        {
            switch (sqlServerType)
            {
                case SqlServerType.BigInt:
                    return typeof(long?);
                case SqlServerType.Bit:
                    return typeof(bool?);
                case SqlServerType.Date:
                    return typeof(DateTime?);
                case SqlServerType.DateTime:
                    return typeof(DateTime?);
                case SqlServerType.Decimal:
                    return typeof(decimal?);
                case SqlServerType.Float:
                    return typeof(double?);
                case SqlServerType.Int:
                    return typeof(int?);
                case SqlServerType.SmallInt:
                    return typeof(short?);
                case SqlServerType.TinyInt:
                    return typeof(byte?);
                case SqlServerType.VarBinary:
                    return typeof(byte?[]);
                case SqlServerType.NText:
                case SqlServerType.NVarChar:
                case SqlServerType.VarChar:
                    return typeof(string);
                default:
                    throw new BaseException($"Type de données non pris en charge: {sqlServerType}");
            }
        }
    }
}