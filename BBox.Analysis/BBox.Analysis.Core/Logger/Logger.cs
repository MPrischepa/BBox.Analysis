using System;
using log4net;

namespace BBox.Analysis.Core.Logger
{
    public class Logger:ILogger 
    {
        private readonly ILog _logger;

        public Logger(ILog logger)
        {
            _logger = logger;
        }

        public void Debug(String format, params object[] args)
        {
            PrepareData();

            _logger?.DebugFormat(format, args);
        }

        public void Error(String message, Exception exception)
        {
            PrepareData();

            _logger?.Error(message, exception);
        }

        public void Error(Exception exception)
        {
            PrepareData();

            _logger?.Error(exception.Message, exception);
        }

        public void Error(String format, params object[] args)
        {
            PrepareData();

            if (_logger == null) return;

            if (args.Length == 1 && args[0] is Exception)
            {
                _logger.Error(format, (Exception)args[0]);
            }
            else
            {
                _logger.ErrorFormat(format, args);
            }
        }

        public void Fatal(object message, Exception exception)
        {
            PrepareData();

            _logger?.Fatal(message, exception);
        }

        public void Info(String format, params object[] args)
        {
            PrepareData();

            _logger?.InfoFormat(format, args);
        }

        public void Warn(String format, params object[] args)
        {
            PrepareData();

            _logger?.WarnFormat(format, args);
        }

        private void PrepareData()
        {

        }
    }
}
