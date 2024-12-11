using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToElasticSearch
{
    [Serializable]
    public class ElasticResultList<T> :List<T>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public long Total { get; set; }
    }

    public static class ElasticResultListExtension
    {
        public static ElasticResultList<T> ToResultList<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                //don't know what to do  (-_-)
                var type = enumerator.GetType();
                var fieldInfo = type.GetField("list", BindingFlags.NonPublic | BindingFlags.Instance);
                return fieldInfo?.GetValue(enumerator) as ElasticResultList<T>;
            }

        }
    }
}
