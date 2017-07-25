using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BBox.Analysis.Domain.RecordTemplates;

namespace BBox.Analysis.Domain
{
  
    public static class BlackBoxObject
    {
        private static readonly IDictionary<Type,IList<object>> _templates = new Dictionary<Type, IList<object>>();
        /// <summary>
        /// Нераспознанные записи
        /// </summary>
        public static IList<Record> UnRecognizedRecords { get; set; }

        static BlackBoxObject()
        {
            UnRecognizedRecords = new List<Record>();
        }

        //internal IEnumerable<RecordTemplate<T>> Templates => _templates;

        public static void AddTemplate<T>(RecordTemplate<T> template) where T:class
        {
            IList<object> __templates;
            if (!_templates.TryGetValue(typeof(T), out __templates))
            {
                __templates = new List<object>();
                _templates.Add(typeof(T),__templates);
            }
            __templates.Add(template);
        }

       
        public static bool ProcessRecord(object entity, Record record) 
        {
            var __templates = GetTemplates(entity);
            if (!__templates.Any())
            {
                UnRecognizedRecords.Add(record);
                return true;
            }
            using (var __enum = __templates.GetEnumerator())
            {
                while (__enum.MoveNext())
                {
                    var __template = __enum.Current;
                    if (__template != null && RecordTemplateExtensions.IsMatch(__template, entity, record))
                        return RecordTemplateExtensions.Process(__template, entity, record);
                }
                UnRecognizedRecords.Add(record);
                return true;
            }
        }

        private static IList<Object> GetTemplates(Object entity)
        {
            var __result = new List<object>();
            var __type = entity.GetType();
            while (__type != null)
            {
                IList<object> __templates;
                _templates.TryGetValue(__type, out __templates);
                if (__templates != null && __templates.Any())
                    __result.AddRange(__templates);
                __type = __type.BaseType;
            }
            return __result;
        }
        public static bool IsMatch(Object entity, Record record)
        {
            var __templates = GetTemplates(entity);
            return __templates.Any() && __templates.Any(x => RecordTemplateExtensions.IsMatch(x, entity, record));
        }
    }
}
