using System;

namespace DirigoEdge.CustomUtils
{
    public static class TimeUtils
    {
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
            var type = obj.GetType();
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType != typeof (DateTime) && prop.PropertyType != typeof (DateTime?))
                {
                    continue;
                }

                var propAsTime = new DateTime();

                if (prop.PropertyType == typeof (DateTime))
                {
                    propAsTime = (DateTime) prop.GetValue(obj, null);

                }

                if (prop.PropertyType == typeof (DateTime?))
                {
                    var nullableTime = (DateTime?) prop.GetValue(obj, null);
                    if (nullableTime.HasValue)
                    {
                        propAsTime = nullableTime.Value;
                    }
                    else
                    {
                        continue;
                    }
                }

                var convertedTime = convertFunction(propAsTime);
                prop.SetValue(obj, convertedTime, null);
            }
        }
    }
}