using System;
using System.Reflection;

namespace BBox.Analysis.Domain.RecordTemplates
{
    public abstract class RecordTemplate<T> where T:class
    {
        public abstract Boolean IsMatch(T entity, Record record);

        public abstract Boolean Process(T entity, Record record);
    }

    public static class RecordTemplateExtensions
    {
        //static MethodInfo _isMatchMethod;

        //static RecordTemplateExtensions()
        //{
        //    _isMatchMethod = typeof(RecordTemplate<>).GetMethod("IsMatch", BindingFlags.Instance | BindingFlags.Public);
        //}

        internal static Boolean IsMatch(object template, Object entity, Record record)
        {
            var __method = template.GetType().GetMethod("IsMatch", BindingFlags.Instance | BindingFlags.Public);
            return (bool)__method.Invoke(template, new[] {entity, record});
        }

        internal static Boolean Process(object template, Object entity, Record record)
        {
            var __method = template.GetType().GetMethod("Process", BindingFlags.Instance | BindingFlags.Public);
            return (bool)__method.Invoke(template, new[] { entity, record });
        }
    }
}
