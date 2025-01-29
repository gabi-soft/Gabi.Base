using Gabi.Base.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Gabi.Test.Data
{
    public class CsvTests
    {
        private const string TestFilePath = "test.csv";

        public CsvTests()
        {
            Cleanup();
        }

        private static void Cleanup()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }

        [Fact]
        public void ReadCsv_ShouldHandleQuotedFieldsCorrectly()
        {
            // Arrange
            var csvContent = "\"Name\";\"Age\";\"Note\"\n\"John\";30;\"Hello; World\"";
            File.WriteAllText(TestFilePath, csvContent);

            var csv = new Csv(separator: ';', quoteChar: '"');

            // Act
            csv.ReadCsv(TestFilePath);

            // Assert
            Assert.Equal(2, csv.Rows.Count);
            Assert.Equal("Hello; World", csv.Rows[1][2]);
        }

        [Fact]
        public void ReadCsv_ShouldThrowExceptionForInvalidFile()
        {
            // Arrange
            var csv = new Csv();

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => csv.ReadCsv("non_existing.csv"));
        }

        [Fact]
        public void WriteCsv_ShouldRespectCustomSeparator()
        {
            // Arrange
            var csv = new Csv(separator: '|');
            csv.Rows.Add(new List<object> { "Name", "Age", "City" });
            csv.Rows.Add(new List<object> { "John", 30, "Paris" });
            csv.Rows.Add(new List<object> { "John\"Junior", 12, "Paris" });

            // Act
            csv.WriteCsv(TestFilePath);

            // Assert
            var content = File.ReadAllText(TestFilePath);
            Assert.Contains("Name|Age|City", content);
            Assert.Contains("John|30|Paris", content);
            Assert.Contains("\"John\"\"Junior\"|12|Paris", content);
        }

        [Fact]
        public void ReadCsv_ShouldParseRowsCorrectly()
        {
            // Arrange
            var csvContent = "Name;Age;IsMember\n\"John\"\"Junior\";30;true\nDoe;\"25\";false";
            File.WriteAllText(TestFilePath, csvContent);

            var csv = new Csv(separator: ';');

            // Act
            csv.ReadCsv(TestFilePath);

            // Assert
            Assert.Equal(3, csv.Rows.Count);
            Assert.Equal("Name", csv.Rows[0][0]);
            Assert.Equal("Age", csv.Rows[0][1]);
            Assert.Equal("IsMember", csv.Rows[0][2]);
            Assert.Equal("John\"Junior", csv.Rows[1][0]);
            Assert.Equal(30.0, csv.Rows[1][1]);
            Assert.Equal(true, csv.Rows[1][2]);
            Assert.Equal("Doe", csv.Rows[2][0]);
            Assert.Equal("25", csv.Rows[2][1]);
            Assert.Equal(false, csv.Rows[2][2]);
        }

        [Fact]
        public void TrimEmptyValues_ShouldHandleRowsWithOnlyEmptyFields()
        {
            // Arrange
            var csv = new Csv(separator: ';');
            csv.Rows.Add(new List<object> { "Name", "Age", "IsMember" });
            csv.Rows.Add(new List<object> { });
            csv.Rows.Add(new List<object> { "John", 30, true });
            csv.Rows.Add(new List<object> { "Doe", 25, false });

            // Act
            csv.TrimEmptyValues();

            // Assert
            Assert.Empty(csv.Rows[1]);
            Assert.Equal(3, csv.Rows[2].Count);
        }

        [Fact]
        public void NormalizeRowLengths_ShouldSupportCustomFillValue()
        {
            // Arrange
            var csv = new Csv();
            csv.Rows.Add(new List<object> { "John" });
            csv.Rows.Add(new List<object> { "Doe", 30 });

            // Act
            csv.NormalizeRowLengths(fillValue: "Missing");

            // Assert
            Assert.Equal("Missing", csv.Rows[0][1]);
        }

        [Fact]
        public void ReadCsv_ShouldHandleEmptyFile()
        {
            // Arrange
            File.WriteAllText(TestFilePath, string.Empty);
            var csv = new Csv();

            // Act
            csv.ReadCsv(TestFilePath);

            // Assert
            Assert.Empty(csv.Rows);
        }

        [Fact]
        public void StressTestCsv()
        {
            // Arrange
            var csv = new Csv();
            var max = short.MaxValue;
            for (var i = 0; i < 100; i++)
            {
                var row = new List<object>(max);
                for (var j = 0; j < 100; j++)
                {
                    row.Add(Guid.NewGuid().ToString());
                }
                csv.Rows.Add(row);
            }

            var filename = "Stress_Test.csv";
            csv.WriteCsv(filename);
            var csv2 = new Csv();
            csv2.ReadCsv(filename);

            // Assert
            Assert.Equal(csv.Rows.Count, csv2.Rows.Count);
            Assert.Equal(csv.Rows[0].Count, csv2.Rows[0].Count);
            Assert.Equal(csv.Rows[1].Count, csv2.Rows[1].Count);
            Assert.Equal(csv.Rows[1][1], csv2.Rows[1][1]);
        }
    }
}
