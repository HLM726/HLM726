using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WorkOrder.Commons
{
    public class IsNulltool
    {
        public static bool AreAllPropertiesNullOrEmpty(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            PropertyInfo[] properties = obj.GetType().GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                if (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType.Equals(typeof(string)))
                {
                    object value = propertyInfo.GetValue(obj);
                    if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
