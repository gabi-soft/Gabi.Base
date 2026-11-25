using System.IO;
using System.Threading.Tasks;
using Gabi.Base.Data;
using Xunit;

namespace Gabi.Test.Data
{
    public class CsvRegressionTests
    {
        [Fact]
        public void ReadCsv_SingleColumnLine_KeepsValue()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "abc\n\"x\"");
                var csv = new Csv();
                csv.ReadCsv(path);

                Assert.Equal("abc", Assert.Single(csv.Rows[0]));
                Assert.Equal("x", Assert.Single(csv.Rows[1]));
            }
            finally
            {
                File.Delete(path);
            }
        }

#if NETCOREAPP2_2_OR_GREATER
        [Fact]
        public async Task ReadCsvAsync_ClearsPreviousRows()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "a;b");
                var csv = new Csv();
                await csv.ReadCsvAsync(path);
                await csv.ReadCsvAsync(path);

                Assert.Single(csv.Rows);
            }
            finally
            {
                File.Delete(path);
            }
        }
#endif
    }
}
