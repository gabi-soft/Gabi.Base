
namespace Gabi.Base.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
#if NET8_0
        private List<object> ParseLine(ReadOnlySpan<char> line)
#else
        private List<object> ParseLine(String line)
#endif
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
            if (value[0] == _quoteChar && value[2] == _quoteChar)
                return value;
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
                int trimIndex = row.Count; // Indique jusqu'où garder les éléments
                for (int i = row.Count - 1; i >= 0; i--)
                {
                    if (row[i] == null || string.IsNullOrWhiteSpace(row[i]?.ToString()))
                    {
                        trimIndex = i; // Met à jour l'indice de coupure
                    }
                    else
                    {
                        trimIndex = row.Count; // Reset si une valeur non vide est rencontrée
                    }
                }
                row.RemoveRange(trimIndex, row.Count - trimIndex); // Supprime les valeurs en excès
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
