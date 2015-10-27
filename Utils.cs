using System;
using System.Collections.Generic;
using System.Linq;

namespace AirSuperiority
{
    public static class Utils
    {
        /// <summary>
        /// Extension for getting a random item from a list
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="items">The list</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this IEnumerable<T> items)
        {
            if (items.Count() < 1) return default(T);
            var random = new Random();
            return (T)(object)items.ToArray()[random.Next(0, items.Count())];
        }

        /// <summary>
        /// Extension for getting a random loot item
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">The list</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this Enum items)
        {
            var types = Enum.GetValues(typeof(T));
            return types.Cast<T>().GetRandomItem();
        }
    }
}
