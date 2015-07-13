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
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                UpdateDatetimeProperty(obj, convertFunction, prop);
            }
        }

        private static bool IsDateTime(PropertyInfo prop)
        {
            return prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?);
        }

        private static void UpdateDatetimeProperty(object obj, Func<DateTime, DateTime> convertFunction, PropertyInfo prop)
        {
            try
            {
                var currentType = prop.PropertyType;
                if (!IsDateTime(prop))
                {
                    return;
                }

                var propAsTime = new DateTime();
                var propertyValue = prop.GetValue(obj, null);

                if (currentType == typeof (DateTime))
                {
                    propAsTime = (DateTime)propertyValue;
                }

                if (currentType == typeof (DateTime?))
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
            catch (Exception ex)
            {
                Log.Warn("Error converting datetime", ex);
            }
        }
    }
}