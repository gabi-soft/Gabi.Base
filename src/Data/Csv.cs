using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gabi.Base.Data
{
    public class Csv
    {
        private readonly char _separator;
        private readonly char _quoteChar;
        public List<List<object>> Rows { get; private set; }

        public Csv(char separator = ';', char quoteChar = '"')
        {
            _separator = separator;
            _quoteChar = quoteChar;
            Rows = new List<List<object>>();
        }

        // Lecture du CSV avec gestion des types
        public void ReadCsv(string filePath)
        {
            Rows.Clear();
            foreach (var line in File.ReadLines(filePath))
            {
                Rows.Add(ParseLine(line));
            }
        }

        // Écriture du CSV avec gestion des types
        public void WriteCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var row in Rows)
                {
                    var rowValues = row
                        .Select(x => x ?? "")
                        .Select(
                            x => (x is string str && ConvertToType(str) is not string)
                                ? $"{_quoteChar}{str}{_quoteChar}"
                                : x?.ToString()
                        );
                    writer.WriteLine(string.Join(_separator.ToString(), row));
                }
            }
        }

        // Parse une ligne CSV avec ReadOnlySpan<char> et gestion des types

        private List<object> ParseLine(string line)
        {
            var fields = new List<object>();
            int currentPos = 0;
            bool insideQuote = false;
            List<char> field = new List<char>();

            while (currentPos < line.Length)
            {
                char currentChar = line[currentPos];

                if (currentChar == _quoteChar)
                {
                    if (insideQuote && currentPos + 1 < line.Length && line[currentPos + 1] == _quoteChar)
                    {
                        // Double quote within quoted field
                        field.Add(currentChar);
                        currentPos++; // Skip the next quote
                    }
                    else
                    {
                        field.Add(currentChar);
                        insideQuote = !insideQuote;
                    }
                }
                else if (currentChar == _separator && !insideQuote)
                {
                    // End of field, convert it to the appropriate type
                    fields.Add(ConvertToType(new string(field.ToArray())));
                    field.Clear();
                }
                else
                {
                    field.Add(currentChar);
                }

                currentPos++;
            }

            // Ajouter le dernier champ
            fields.Add(ConvertToType(new string(field.ToArray())));
            return fields;
        }

        // Convertir une chaîne en un type spécifique
        private object ConvertToType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (value.Length >= 3 && value[0] == _quoteChar && value[value.Length - 1] == _quoteChar)
                return value.Substring(1, value.Length - 2);
            if (double.TryParse(value, out var doubleValue))
                return doubleValue;
            if (int.TryParse(value, out var intValue))
                return intValue;
            if (bool.TryParse(value, out var boolValue))
                return boolValue;
            if (DateTime.TryParse(value, out var dateTimeValue))
                return dateTimeValue;
            // Pour les autres types, on renvoie la chaîne telle quelle
            return value;
        }

        public void TrimEmptyValues()
        {
            foreach (var row in Rows)
            {
                row.RemoveAll(static x => string.IsNullOrWhiteSpace(x?.ToString() ?? ""));
            }
        }

        public void NormalizeRowLengths(object fillValue = null)
        {
            if (Rows.Count == 0) return;

            // Déterminer la longueur maximale
            int maxLength = Rows.Max(row => row.Count);

            foreach (var row in Rows)
            {
                while (row.Count < maxLength)
                {
                    row.Add(fillValue);
                }
            }
        }
    }
}
