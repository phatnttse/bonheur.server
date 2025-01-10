using System;
using System.Collections.Generic;
using System.Reflection;

namespace MiddlewareTool.Utility
{
    /// <summary>
    /// ReflectorUtility
    /// </summary>
    public class ReflectorUtility
    {
        /// <summary>
        /// Follow Property Path
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="path">path</param>
        /// <returns></returns>
        public static object FollowPropertyPath(object value, string path)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (path == null) { throw new ArgumentNullException("path"); }

            Type currentType = value.GetType();

            object obj = value;
            foreach (string propertyName in path.Split('.'))
            {
                if (currentType != null)
                {
                    PropertyInfo property = null;
                    int brackStart = propertyName.IndexOf("[");
                    int brackEnd = propertyName.IndexOf("]");

                    property = currentType.GetProperty(brackStart > 0 ? propertyName.Substring(0, brackStart) : propertyName);
                    if (property != null)
                    {
                        obj = property.GetValue(obj, null);

                        if (brackStart > 0)
                        {
                            string index = propertyName.Substring(brackStart + 1, brackEnd - brackStart - 1);
                            foreach (Type iType in obj.GetType().GetInterfaces())
                            {
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                                {
                                    obj = typeof(ReflectorUtility).GetMethod("GetDictionaryElement")
                                                         .MakeGenericMethod(iType.GetGenericArguments())
                                                         .Invoke(null, new object[] { obj, index });
                                    break;
                                }
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                                {
                                    obj = typeof(ReflectorUtility).GetMethod("GetListElement")
                                        .MakeGenericMethod(iType.GetGenericArguments())
                                        .Invoke(null, new object[] { obj, index });
                                    break;
                                }
                            }
                        }

                        currentType = obj?.GetType();
                    }
                    else { return null; }
                }
                else { return null; }
            }
            return obj;
        }
        /// <summary>
        /// Get Dictionary Element
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dict">dict</param>
        /// <param name="index">index</param>
        /// <returns></returns>
        public static TValue GetDictionaryElement<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            TKey key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }
        ///// <summary>
        ///// GetListElement
        ///// </summary>
        ///// <typeparam name="T">T</typeparam>
        ///// <param name="list">list</param>
        ///// <param name="index">index</param>
        ///// <returns></returns>
        ////public static T GetListElement<T>(IList<T> list, object index)
        ////{
        ////    int m_Index = Convert.ToInt32(index);
        ////    T m_T = list.Count > m_Index ? list[m_Index] : default(T);

        ////    return m_T;
        ////}
    }
}
