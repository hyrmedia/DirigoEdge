using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.CustomUtils
{
    public static class TimeUtils
    {
        public static readonly ILog Log = LogFactory.GetLog(typeof (TimeUtils));


        public static DateTime ConvertLocalToUTC(DateTime time)
        {
            return time.Subtract(SystemTime.LocalOffset);
        }

        public static DateTime ConvertUTCToLocal(DateTime time)
        {
            return time.Add(SystemTime.LocalOffset);
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
        {foreach (var prop in obj.GetType().GetProperties())
            {
                var itemType = prop.PropertyType;

                if (IsDateTime(itemType))
                {
                    ConvertSingleDateTime(obj, convertFunction, prop);
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(itemType))
                {
                    ConvertDatesInList(obj, prop, convertFunction);
                    continue;
                }
                
                if (IsDirigoClass(itemType))
                {
                    ConvertAllMembers(prop.GetValue(obj, null), convertFunction);
                }
            }
        }

        private static void ConvertDatesInList(object obj, PropertyInfo prop, Func<DateTime, DateTime> convertFunction)
        {
            try
            {
                var itemlist = ((IEnumerable<Object>) prop.GetValue(obj, null)).ToList();

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
                }


                if (IsDateTime(listType))
                {
                    if (prop.GetType() != typeof (List<>))
                    {
                      
                        // At this point I couldn't figure out a way to instantiate a new 
                        // list/array/etc dynamically without a huge switch for each type.
                        // for now, we support lists and can other types as need be.
                        return;
                    }

                    //     var newDateList = new List<DateTime>()
                    //      l
                    //   prop.SetValue();
                    foreach (var item in itemlist)
                    {
                        // item = convertFunction((DateTime) item);
                    }
                }
            }
            catch 
            {
                
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

            if (currentType == typeof (DateTime))
            {
                propAsTime = (DateTime) propertyValue;
            }

            if (currentType == typeof (DateTime?))
            {
                var nullableTime = (DateTime?) propertyValue;
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
    }
}