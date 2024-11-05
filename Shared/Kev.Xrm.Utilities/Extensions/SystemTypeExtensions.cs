using System;
using Microsoft.Xrm.Sdk;

namespace Kev.Xrm.Utilities.Extensions
{
    public static class SystemTypeExtensions
    {
        public static bool IsTrue(this bool? nullableBoolean)
        {
            return nullableBoolean.HasValue && nullableBoolean.Value;
        }

        public static EntityReference ToEntityReference(this Guid id, string logicalName)
        {
            return new EntityReference(logicalName, id);
        }

        public static EntityReference ToEntityReference<T>(this Guid id) where T : Entity
        {
            var logicalName = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            return new EntityReference(logicalName, id);
        }
    }
}