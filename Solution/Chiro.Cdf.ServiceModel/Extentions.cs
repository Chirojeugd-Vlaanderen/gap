using System;
using System.Collections.Generic;
using System.Linq;

namespace Chiro.Cdf.ServiceModel
{
    public static class CollectionExtentions
    {
        public static IEnumerable<U> ConvertAll<T, U>(this IEnumerable<T> collection, Converter<T, U> converter)
        {
            return collection.Select(item => converter(item));
        }
    }

    public static class ArrayExtentions
    {
        public static U[] ConvertAll<T, U>(this T[] array, Converter<T, U> converter)
        {
            IEnumerable<T> enumerable = array;
            return enumerable.ConvertAll(converter).ToArray();
        }
    }
}
