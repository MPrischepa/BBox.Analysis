using System;
using System.Reflection;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates
{
    public abstract class RecordTemplate<T> where T:class
    {
        public abstract Boolean IsMatch(T entity, Record record);

        public abstract ProcessingResult Process(T entity, Record record);
    }

    public static class RecordTemplateExtensions
    {
        //static MethodInfo _isMatchMethod;

        //static RecordTemplateExtensions()
        //{
        //    _isMatchMethod = typeof(RecordTemplate<>).GetMethod("IsMatch", BindingFlags.Instance | BindingFlags.Public);
        //}

        internal static Boolean IsMatch(object template, Entity entity, Record record)
        {
            var __method = template.GetType().GetMethod("IsMatch", BindingFlags.Instance | BindingFlags.Public);
            return (bool)__method.Invoke(template, new object[] {entity, record});
        }

        internal static ProcessingResult Process(object template, Entity entity, Record record)
        {
            
            var __method = template.GetType().GetMethod("Process", BindingFlags.Instance | BindingFlags.Public);
            var __result = (ProcessingResult)__method.Invoke(template, new object[] { entity, record });
            if (__result == ProcessingResult.SelfProcessing && ProcessingSettings.Instatnce.BuildGapCounterReports)
                entity.AddRecord(record);
            return __result;
        }
    }
}
