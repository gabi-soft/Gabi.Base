using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        // Lecture du CSV avec gestion des types
        public void ReadCsv(string filePath)
        {
            Rows.Clear();
            foreach (var line in File.ReadLines(filePath))
            {
                var lineSpan = line.AsSpan();
                if (lineSpan.Length == 0)
                    Rows.Add(new List<object>());
                else
                    Rows.Add(ParseLine(lineSpan));
            }
        }

        // Écriture du CSV avec gestion des types
        public void WriteCsv(string filePath, bool append = false, Encoding encoding = null)
        {
            encoding ??= Encoding.Default;
            using var writer = new StreamWriter(filePath, false, encoding);

            foreach (var row in Rows)
            {
                if (row.Count == 0) continue;
                var rowValues = row
                    .Select(
                        x => x is string str && (ConvertToType(str) is not string || str.Contains(_quoteChar))
                            ? $"{_quoteChar}{str.Replace($"{_quoteChar}", $"{_quoteChar}{_quoteChar}")}{_quoteChar}"
                            : x?.ToString()
                    );
                writer.WriteLine(string.Join(_separator.ToString(), rowValues));
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
            var strValue = value;
            if (value.Length == 0)
                return null;
            if (value.Length >= 3 && value[0] == _quoteChar && value[value.Length - 1] == _quoteChar)
                return value.Slice(1, value.Length - 2).ToString();
            if (double.TryParse(strValue, out var doubleValue))
                return doubleValue;
            if (int.TryParse(strValue, out var intValue))
                return intValue;
            if (bool.TryParse(strValue, out var boolValue))
                return boolValue;
            if (DateTime.TryParse(strValue, out var dateTimeValue))
                return dateTimeValue;
            // Pour les autres types, on renvoie la chaîne telle quelle
            return value.ToString();
        }

        public void TrimEmptyValues()
        {
            foreach (var row in Rows) row.RemoveAll(static x => x == null || x == string.Empty);
        }

        public void NormalizeRowLengths(object fillValue = null)
        {
            if (Rows.Count == 0) return;

            // Déterminer la longueur maximale
            var maxLength = Rows.Max(row => row.Count);

            foreach (var row in Rows)
                while (row.Count < maxLength)
                    row.Add(fillValue);
        }
    }
}