using System;

namespace Data.Interfaces
{
    public interface IDiagnosticsLogger : IDisposable
    {
        void LogMessage(string message);
    }
}