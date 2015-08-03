using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.CustomUtils
{
    public static class TimeUtils
    {
        public static readonly ILog Log = LogFactory.GetLog(typeof(TimeUtils));

        public static String GetTimeConvertedDynamicHtml(string partialViewName, object model)
        {
            ConvertAllMembersToLocal(model);
            return DynamicModules.GetViewHtml(partialViewName, model);
        }

        public static DateTime ConvertLocalToUTC(DateTime time)
        {
            var convertTime =  time.Subtract(SystemTime.LocalOffset);
           
            if (SystemTime.LocalTimeZone.IsDaylightSavingTime(convertTime))
            {
                convertTime = convertTime.AddHours(-1);
            }

            return convertTime;
        }

        public static DateTime ConvertUTCToLocal(DateTime time)
        {
            var convertTime = time.Add(SystemTime.LocalOffset);

            if (SystemTime.LocalTimeZone.IsDaylightSavingTime(convertTime))
            {
                convertTime = convertTime.AddHours(1);
            }

            return convertTime;
        }

        public static void ConvertAllMembersToLocal(Object obj)
        {
            ConvertAllMembers(obj, ConvertUTCToLocal);
        }

        public static void ConvertAllMembersToUTC(Object obj)
        {
            ConvertAllMembers(obj, ConvertLocalToUTC);
        }

        private static void ConvertAllMembers(Object obj, Func<DateTime, DateTime> convertFunction)
        {
            if (obj == null)
            {
                return;
            }

            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic   | BindingFlags.Instance  | BindingFlags.FlattenHierarchy))
            {
                var itemType = prop.PropertyType;

                if (IsDateTime(itemType))
                {
                    ConvertSingleDateTime(obj, convertFunction, prop);
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(itemType))
                {
                    ConvertList(obj, prop, convertFunction);
                    continue;
                }

                if (IsDirigoClass(itemType))
                {
                    ConvertAllMembers(prop.GetValue(obj, null), convertFunction);
                }
            }
        }

        private static void ConvertList(object obj, PropertyInfo prop, Func<DateTime, DateTime> convertFunction)
        {
            try
            {
                var itemlist = ((IEnumerable<Object>)prop.GetValue(obj, null)).ToList();

                if (itemlist.Count == 0)
                {
                    return;
                }

                var listType = itemlist.First().GetType();

                if (IsDirigoClass(listType))
                {
                    foreach (var item in itemlist)
                    {
                        ConvertAllMembers(item, convertFunction);
                    }
                    return;
                }

                if (IsDateTime(listType))
                {
                    ConvertDateList(obj, prop, convertFunction, listType, itemlist);
                    return;
                }
            }
            catch
            {

            }
        }

        private static void ConvertDateList(object obj, PropertyInfo prop, Func<DateTime, DateTime> convertFunction, Type listType, List<object> itemlist)
        {
            if (prop.GetType() != typeof(List<>))
            {
                // At this point I couldn't figure out a way to instantiate a new 
                // list/array/etc dynamically without a huge switch for each type.
                // for now, we support lists and can other types as need be.
                return;
            }

            if (listType == typeof(DateTime))
            {
                UpdateDateTimeList(obj, prop, convertFunction, itemlist);
            }

            if (listType == typeof(DateTime?))
            {
                UpdateNullableDateTimeList(obj, prop, convertFunction, itemlist);
            }
        }

        private static void UpdateDateTimeList(object obj, PropertyInfo prop, Func<DateTime, DateTime> convertFunction, List<object> itemlist)
        {
            var newList = new List<DateTime>();

            foreach (DateTime dateItem in itemlist)
            {
                newList.Add(convertFunction(dateItem));
            }

            prop.SetValue(obj, newList, null);
        }

        private static void UpdateNullableDateTimeList(object obj, PropertyInfo prop, Func<DateTime, DateTime> convertFunction, List<object> itemlist)
        {
            var newList = new List<DateTime?>();
            foreach (DateTime? dateItem in itemlist)
            {
                if (dateItem.HasValue)
                {
                    newList.Add(convertFunction(dateItem.Value));
                }
                else
                {
                    newList.Add(null);
                }

                prop.SetValue(obj, newList, null);
            }
        }

        private static bool IsDirigoClass(Type itemType)
        {
            return itemType.FullName.ToLower().Contains("dirigo");
        }

        private static bool IsDateTime(Type prop)
        {
            return prop == typeof(DateTime) || prop == typeof(DateTime?);
        }

        private static void ConvertSingleDateTime(object obj, Func<DateTime, DateTime> convertFunction, PropertyInfo prop)
        {
            var currentType = prop.PropertyType;
            var propAsTime = new DateTime();
            var propertyValue = prop.GetValue(obj, null);

            if (currentType == typeof(DateTime))
            {
                propAsTime = (DateTime)propertyValue;
            }

            if (currentType == typeof(DateTime?))
            {
                var nullableTime = (DateTime?)propertyValue;
                if (nullableTime.HasValue)
                {
                    propAsTime = nullableTime.Value;
                }
                else
                {
                    return;
                }
            }

            var convertedTime = convertFunction(propAsTime);
            prop.SetValue(obj, convertedTime, null);
        }

        public static DateTime? ConvertUTCToLocal(DateTime? nullable)
        {
            return nullable.HasValue
                ? (DateTime?) ConvertUTCToLocal(nullable.Value) 
                : null;
        }

        public static DateTime? ConvertLocalToUTC(DateTime? nullable)
        {
            return nullable.HasValue 
                ? (DateTime?) ConvertLocalToUTC(nullable.Value) 
                : null;
        }
    }
}