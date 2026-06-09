using System.Collections.Concurrent;
using System.Text;
using Data.Interfaces;

namespace Data.Entities
{
    public sealed class DiagnosticsLogger : IDiagnosticsLogger
    {
        private readonly string _filePath;
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly Task _writerTask;
        private readonly CancellationTokenSource _cts = new();

        public DiagnosticsLogger()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

            _filePath = $"Diagnostics_{timestamp}_{uniqueId}.log";

            _writerTask = Task.Factory.StartNew(
                WriteLoop,
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        // PRODUCER: Physics threads call this, it never blocks
        public void LogMessage(string message)
        {
            if (!_cts.Token.IsCancellationRequested)
            {
                var log = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
                _logQueue.Enqueue(log);
            }
        }

        // CONSUMER: Writes to the file at its own pace
        private async Task WriteLoop()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_logQueue.IsEmpty)
                    {
                        await Task.Delay(50, _cts.Token);
                        continue;
                    }
                    
                    using var writer = new StreamWriter(_filePath, append: true, Encoding.ASCII);
                    
                    while (_logQueue.TryDequeue(out var logMessage))
                    {
                        await writer.WriteLineAsync(logMessage);
                    }
                }
            }
            catch (TaskCanceledException) {}
            catch (Exception) {}
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _writerTask.Wait(); } catch { }
            _cts.Dispose();
        }
    }
}