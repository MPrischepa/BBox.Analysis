using System;
using System.Collections.Generic;
using System.IO;
using BBox.Analysis.Domain.PaymentTypes;
using BBox.Analysis.Interface;

namespace BBox.Analysis.Processing
{
    public class BlackBoxProcessingManager
    {
        private readonly String _inDirectory;
        private String _searchTemplate = "BBOX*.txt";

        private ILogger _logger;
        private IRegistrar _registrar;

        private void WriteLog(String text)
        {
            _logger?.Write(text);
        }

        public BlackBoxProcessingManager(String inDirectory)
        {
            _inDirectory = inDirectory;
        }

        public BlackBoxProcessingManager WithSearchTemplate(String searchTemplate)
        {
            _searchTemplate = searchTemplate;
            return this;
        }

        public BlackBoxProcessingManager WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public BlackBoxProcessingManager WithRegistrar(IRegistrar registrar)
        {
            _registrar = registrar;
            PaymentFactory.CreateInstance(_registrar,_registrar);
            return this;
        }

        public BlackBoxProcessingManager WithPayments(IList<PaymentConfig> payments)
        {
            PaymentFactory.Instance.Configure(payments);
            return this;
        }

        public void Process()
        {
            var __inFiles = Directory.GetFiles(_inDirectory, _searchTemplate);
            foreach (var __t in __inFiles)
            {
                WriteLog($"{DateTime.Now}: Обработка: {__t}");
                var __fileProcessor = new FileProcessor(_logger, _registrar);
                __fileProcessor.ProcessFile(__t);
                WriteLog($"{DateTime.Now}: Обработка завершена: {__t}");
            }

            _registrar.AfterProcessDataFinished();
        }
    }
}
