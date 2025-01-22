using Gabi.Base.Data;
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

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }

        [Fact]
        public void ReadCsv_ShouldParseRowsCorrectly()
        {
            // Arrange
            var csvContent = "Name;Age;IsMember\nJohn;30;true\nDoe;\"25\";false";
            File.WriteAllText(TestFilePath, csvContent);

            var csv = new Csv(separator: ';');

            // Act
            csv.ReadCsv(TestFilePath);

            // Assert
            Assert.Equal(3, csv.Rows.Count);
            Assert.Equal("Name", csv.Rows[0][0]);
            Assert.Equal("Age", csv.Rows[0][1]);
            Assert.Equal("IsMember", csv.Rows[0][2]);
            Assert.Equal("John", csv.Rows[1][0]);
            Assert.Equal(30.0, csv.Rows[1][1]);
            Assert.Equal(true, csv.Rows[1][2]);
            Assert.Equal("Doe", csv.Rows[2][0]);
            Assert.Equal("25", csv.Rows[2][1]);
            Assert.Equal(false, csv.Rows[2][2]);
        }

        [Fact]
        public void WriteCsv_ShouldWriteRowsCorrectly()
        {
            // Arrange
            var csv = new Csv(separator: ';');
            csv.Rows.Add(new List<object> { "Name", "Age", "IsMember" });
            csv.Rows.Add(new List<object> { "John", 30, true });
            csv.Rows.Add(new List<object> { "Doe", 25, false });

            // Act
            csv.WriteCsv(TestFilePath);

            // Assert
            var writtenContent = File.ReadAllText(TestFilePath);
            var expectedContent = "Name;Age;IsMember\r\nJohn;30;True\r\nDoe;25;False\r\n";
            Assert.Equal(expectedContent, writtenContent);
        }

        [Fact]
        public void TrimEmptyValues_ShouldRemoveTrailingEmptyFields()
        {
            // Arrange
            var csv = new Csv();
            csv.Rows.Add(new List<object> { "John", 30, null, null });
            csv.Rows.Add(new List<object> { "Doe", null, null });

            // Act
            csv.TrimEmptyValues();

            // Assert
            Assert.Equal(2, csv.Rows[0].Count);
            Assert.Equal(1, csv.Rows[1].Count);
        }

        [Fact]
        public void NormalizeRowLengths_ShouldMakeAllRowsEqualLength()
        {
            // Arrange
            var csv = new Csv();
            csv.Rows.Add(new List<object> { "John", 30 });
            csv.Rows.Add(new List<object> { "Doe" });

            // Act
            csv.NormalizeRowLengths(fillValue: "N/A");

            // Assert
            Assert.Equal(2, csv.Rows.Count);
            Assert.Equal(2, csv.Rows[0].Count);
            Assert.Equal(2, csv.Rows[1].Count);
            Assert.Equal("N/A", csv.Rows[1][1]);
        }
    }
}
