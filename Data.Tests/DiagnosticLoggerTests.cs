using Data.Entities;
using Data.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Data.Tests
{
    public class DiagnosticLoggerTests
    {
        [Fact]
        public async Task Logger_ShouldNotBlockThread_AndShouldWriteToFileAsynchronously()
        {
            var directory = Directory.GetCurrentDirectory();
            var filesBefore = Directory.GetFiles(directory, "Diagnostics_*.log");

            var logger = new DiagnosticsLogger();
            var stopwatch = Stopwatch.StartNew();
            int logsCount = 1000;

            for (int i = 0; i < logsCount; i++)
            {
                logger.LogMessage($"Test diagnostic message {i}");
            }

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 100,
                $"Logger działa za wolno ({stopwatch.ElapsedMilliseconds} ms) i blokowałby wątki fizyki!");

            await Task.Delay(500);

            logger.Dispose();

            var filesAfter = Directory.GetFiles(directory, "Diagnostics_*.log");
            var myLogFile = filesAfter.Except(filesBefore).FirstOrDefault();

            Assert.NotNull(myLogFile);
            Assert.True(File.Exists(myLogFile));

            string[] fileLines = Array.Empty<string>();
            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    fileLines = await File.ReadAllLinesAsync(myLogFile);
                    break;
                }
                catch (IOException)
                {
                    retries--;
                    await Task.Delay(100);
                }
            }

            Assert.True(fileLines.Length >= logsCount, $"Plik zapisał tylko {fileLines.Length} logów zamiast {logsCount}!");

            try
            {
                File.Delete(myLogFile);
            }
            catch {}
        }
    }
}