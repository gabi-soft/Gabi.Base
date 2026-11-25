using Gabi.Base.Sql;
using Gabi.Base.Utils;
using Xunit;

namespace Gabi.Test.Utils
{
    public class UtilsTests
    {
        [Fact]
        public void LogBuffer_Print_WithBracesInMessage_DoesNotThrowAndReturnsText()
        {
            var buffer = new LogBuffer(Serilog.Events.LogEventLevel.Warning);
            buffer.Add("valeur {0} et {Inconnu}");
            Assert.Contains("{Inconnu}", buffer.Print());
            Assert.False(buffer.HasLogs());
        }

        [Fact]
        public void LogBuffer_Print_WhenEmpty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, new LogBuffer(Serilog.Events.LogEventLevel.Warning).Print());
        }

        [Fact]
        public void SqlObjectName_Null_IsInvalid()
        {
            Assert.False(SqlObjectName.IsValidSqlObjectName((string)null));
            Assert.False(SqlObjectName.IsValidSqlObjectName("a]b"));
            Assert.True(SqlObjectName.IsValidSqlObjectName("Ma table"));
        }

        [Fact]
        public void FilenameTemplate_ReplacesTokens()
        {
            var name = FilenameTemplateProcessor.Replace("$CONF$-$DATE$.log", "app");
            Assert.StartsWith("app-", name);
            Assert.DoesNotContain("$", name);
        }
    }
}
