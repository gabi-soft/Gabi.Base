using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabi.Base.Data
{
    public class Csv
    {
        private readonly char _quoteChar;
        private readonly char _separator;

        public Csv(char separator = ';', char quoteChar = '"')
        {
            _separator = separator;
            _quoteChar = quoteChar;
            Rows = new List<List<object>>();
        }

        public List<List<object>> Rows { get; }

        /// <summary>
        ///     Lecture du CSV
        /// </summary>
        /// <param name="filePath">Chemin de CSV</param>
        public void ReadCsv(string filePath)
        {
            Rows.Clear();
            foreach (var line in File.ReadLines(filePath))
            {
                var lineSpan = line.AsSpan();
                Rows.Add(lineSpan.Length == 0 ? new List<object>() : ParseLine(lineSpan));
            }
        }

#if NETCOREAPP2_2_OR_GREATER
        /// <summary>
        ///     Lecture du CSV asynchrone
        /// </summary>
        /// <param name="filePath">Chemin de CSV</param>
        public async Task ReadCsvAsync(string filePath)
        {
            await foreach (var line in File.ReadLinesAsync(filePath))
                Rows.Add(line.Length == 0 ? new List<object>() : ParseLine(line));
        }
#endif

        /// <summary>
        ///     Écriture du CSV
        /// </summary>
        /// <param name="filePath">Chemin de CSV</param>
        /// <param name="append">Ajouter à la suite</param>
        /// <param name="encoding">Encodage du fichier</param>
        public void WriteCsv(string filePath, bool append = false, Encoding encoding = null)
        {
            encoding ??= Encoding.Default;
            using var writer = new StreamWriter(filePath, append, encoding);

            foreach (var row in Rows)
            {
                if (row.Count == 0) continue;
                var rowValues = FormatRowStringValues(row);
                writer.WriteLine(string.Join(_separator.ToString(), rowValues));
            }
        }

        /// <summary>
        ///     Écriture du CSV asynchrone
        /// </summary>
        /// <param name="filePath">Chemin de CSV</param>
        /// <param name="append">Ajouter à la suite</param>
        /// <param name="encoding">Encodage du fichier</param>
        public async Task WriteCsvAsync(string filePath, bool append = false, Encoding encoding = null)
        {
            encoding ??= Encoding.Default;
            using var writer = new StreamWriter(filePath, append, encoding);

            foreach (var row in Rows)
            {
                if (row.Count == 0) continue;
                var rowValues = FormatRowStringValues(row);
                await writer.WriteLineAsync(string.Join(_separator.ToString(), rowValues));
            }
        }

        private List<object> ParseLine(ReadOnlySpan<char> line)
        {
            var nbField = line.Count(_separator) + 1;
            var fields = new List<object>(nbField);
            if (line.Length == 0) return fields;

            var currentPos = 0;
            var insideQuote = false;

            var field = new char[line.Length].AsSpan();
            var fieldPos = 0;

            while (currentPos < line.Length && nbField > 1)
            {
                var currentChar = line[currentPos];

                if (currentChar == _quoteChar)
                {
                    if (insideQuote && currentPos + 1 < line.Length && line[currentPos + 1] == _quoteChar)
                    {
                        field[fieldPos] = currentChar;
                        currentPos++;
                    }
                    else
                    {
                        field[fieldPos] = currentChar;
                        insideQuote = !insideQuote;
                    }

                    fieldPos++;
                }
                else if (currentChar == _separator && !insideQuote)
                {
                    fields.Add(ConvertToType(field.Slice(0, fieldPos)));
                    fieldPos = 0;
                }
                else
                {
                    field[fieldPos] = currentChar;
                    fieldPos++;
                }

                currentPos++;
            }

            // Ajouter le dernier champ
            fields.Add(ConvertToType(field.Slice(0, fieldPos)));
            return fields;
        }

        // Convertir une chaîne en un type spécifique
        private object ConvertToType(ReadOnlySpan<char> value)
        {
            if (value.Length == 0)
                return null;
            if (value.Length >= 3 && value[0] == _quoteChar && value[value.Length - 1] == _quoteChar)
                return value.Slice(1, value.Length - 2).ToString().Replace("\\n", "\n");
            if (double.TryParse(value, out var doubleValue))
                return doubleValue;
            if (int.TryParse(value, out var intValue))
                return intValue;
            if (bool.TryParse(value, out var boolValue))
                return boolValue;
            if (DateTime.TryParse(value, out var dateTimeValue))
                return dateTimeValue;
            // Pour les autres types, on renvoie la chaîne telle quelle
            return value.ToString().Replace("\\n", "\n");
        }

        /// <summary>
        ///     Supprime les valeurs vides (null ou chaîne vide) de chaque ligne dans <see cref="Rows" />.
        /// </summary>
        public void TrimEmptyValues()
        {
            foreach (var row in Rows) row.RemoveAll(static x => x == null || x == string.Empty);
        }

        /// <summary>
        ///     Normalise la longueur des lignes dans <see cref="Rows" /> en les remplissant avec <paramref name="fillValue" />
        ///     jusqu'à atteindre la longueur maximale.
        /// </summary>
        /// <param name="fillValue">La valeur utilisée pour remplir les lignes plus courtes. Par défaut, <c>null</c>.</param>
        public void NormalizeRowLengths(object fillValue = null)
        {
            if (Rows.Count == 0) return;

            // Déterminer la longueur maximale
            var maxLength = Rows.Max(row => row.Count);

            foreach (var row in Rows)
                while (row.Count < maxLength)
                    row.Add(fillValue);
        }

        /// <summary>
        ///     Formate les valeurs d'une ligne en les échappant si nécessaire.
        ///     Les chaînes sont entourées de guillemets et les caractères spéciaux sont échappés.
        /// </summary>
        /// <param name="row">La collection d'objets à formater.</param>
        /// <returns>Une séquence de chaînes formatées représentant les valeurs de la ligne.</returns>
        private IEnumerable<string?> FormatRowStringValues(IEnumerable<object?> row)
        {
            foreach (var x in row)
            {
                if (x is string str)
                {
                    var converted = ConvertToType(str);
                    if (converted is not string || str.Contains(_quoteChar))
                    {
                        var escaped = str
                            .Replace($"{_quoteChar}", $"{_quoteChar}{_quoteChar}")
                            .Replace("\n", "\\n");
                        yield return $"{_quoteChar}{escaped}{_quoteChar}";
                        continue;
                    }
                }

                yield return x?.ToString()?.Replace("\n", "\\n");
            }
        }
    }
}