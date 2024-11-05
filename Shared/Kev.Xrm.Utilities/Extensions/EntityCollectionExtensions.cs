using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Kev.Xrm.Utilities.Extensions
{
    public static class EntityCollectionExtensions
    {
        /// <summary>
        /// Gets a list of typed entities
        /// </summary>
        /// <typeparam name="T">Type of the entity to get</typeparam>
        /// <param name="collection">Original Late Bound Entities collection</param>
        /// <returns>List of typed entitie</returns>
        public static List<T> ToEntities<T>(this EntityCollection collection) where T:Entity
        {
           return collection.Entities.Select(e => e.ToEntity<T>()).ToList();
        }
    }
}
