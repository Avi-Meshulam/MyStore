using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using System.Reflection;
using MyStore.BL.Models;

namespace MyStore.BL
{
    public static class Extensions
    {
        /// <summary>
        /// Iterates over the collection, adds new items and attaches existing ones
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="collection"></param>
        public static void Sync<T>(this DbSet<T> entity,
            ICollection<T> collection) where T : class, ICollectionItem
        {
            // If there are items in collection that don't exist in entity - add them. otherwise: update.
            foreach (var item in collection)
            {
                if (!entity.Any(i => i.Equals(item)))
                {
                    entity.Add(item);
                }
                else
                    entity.Update(item);
            }

            uint parentId = 0;
            if (collection?.Count > 0)
                parentId = collection.First().ParentId;
            else if (entity.Count() > 0)
                parentId = entity.First().ParentId;

            // If there are items in entity that don't exist in collection - delete them
            foreach (var item in entity)
            {
                if ((item as ICollectionItem).ParentId == parentId
                    && ((collection?.Count ?? 0) == 0 || !collection.Any(i => i.Equals(item))))
                    entity.Remove(item);
            }
        }

        /// <summary>
        /// Iterates over the collection, adds new items and attaches existing ones
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="collection"></param>
        /// <param name="keySelector"></param>
        public static void Sync<T, TKey>(this DbSet<T> entity,
            ICollection<T> collection, Func<T, TKey> keySelector) where T : class, ICollectionItem
        {
            foreach (var item in collection)
            {
                if (!entity.Any(i => keySelector(i).Equals(keySelector(item))))
                {
                    entity.Add(item);
                }
                else
                    entity.Update(item);
            }

            uint parentId = 0;
            if (collection?.Count > 0)
                parentId = collection.First().ParentId;
            else if (entity.Count() > 0)
                parentId = entity.First().ParentId;

            foreach (var item in entity)
            {
                if ((item as ICollectionItem).ParentId == parentId
                    && ((collection?.Count ?? 0) == 0
                        || !collection.Any(i => keySelector(i).Equals(keySelector(item)))))
                    entity.Remove(item);
            }
        }

        public static string SplitCamelCase(this string input)
        {
            var regex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            string output = regex.Replace(input, " ");

            return $"{char.ToUpper(output[0])}{output.Substring(1)}";
        }
    }
}
