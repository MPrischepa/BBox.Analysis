using System;

namespace BBox.Analysis.Core.Logger
{
    public interface ILogger
    {
        void Debug(string format, params object[] args);

        void Error(string message, Exception exception);

        void Error(Exception exception);

        void Error(string format, params object[] args);

        void Fatal(object message, Exception exception);

        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);
    }
}
