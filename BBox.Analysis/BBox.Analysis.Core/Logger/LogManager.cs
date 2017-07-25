using System;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net.Appender;

namespace BBox.Analysis.Core.Logger
{
    public class LogManager
    {
        private static LogManager _instance;

        private LogManager()
        {
            Initialize();
        }

        public static LogManager GetInstance()
        {
            lock (InitObject)
            {
                if (_instance != null) return _instance;
                _instance = new LogManager();
            }
            return _instance;
        }

        private bool _initialize;
        private static readonly object InitObject = new object();

        private void Initialize()
        {
            lock (InitObject)
            {
                var __configpath = ConfigurationManager.AppSettings["LogConfigPath"];
                if (string.IsNullOrEmpty(__configpath))
                    __configpath = "log.config";
                var __file = new FileInfo(__configpath);
                if (!__file.Exists)
                {
                    __configpath = Path.Combine(Path.Combine(".", "config"), __configpath);
                    __file = new FileInfo(__configpath);
                }
                log4net.Config.XmlConfigurator.ConfigureAndWatch(__file);

                log4net.ThreadContext.Properties["UserAddress"] = null;
                log4net.ThreadContext.Properties["UserName"] = null;

                _initialize = true;
            }
        }

        public void Configure(String configPath)
        {
            lock (InitObject)
            {
                _initialize = false;
                var __file = new FileInfo(configPath);
                log4net.Config.XmlConfigurator.ConfigureAndWatch(__file);
                _initialize = true;
            }
        }

        public ILogger GetLogger(string name)
        {
            if (!_initialize)
            {
                Initialize();
            }
            return new Core.Logger.Logger(log4net.LogManager.GetLogger(name));
        }

        public void Flush()
        {
            var __rep = log4net.LogManager.GetRepository();
            foreach (var __buffered in __rep.GetAppenders().OfType<BufferingAppenderSkeleton>())
            {
                __buffered.Flush();
            }
        }
    }
}
