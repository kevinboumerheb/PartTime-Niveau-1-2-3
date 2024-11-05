using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kev.Xrm.Utilities.Extensions
{
    public static class EntityExtensions
    {
        public static Entity Clone(this Entity record, IOrganizationService service, bool addCloneLabel = true, params string[] attributesToRemove)
        {
            var standardToRemove = new[] { "createdon", "createdonbehalfby", "createdby", "modifiedon", "modifiedonbehalfby", "modifiedby", "overridencreatedon", "overridencreatedby", "ownerid" };
            var toRemove = standardToRemove.Concat(attributesToRemove).ToArray();

            // Recherche des m�tadonn�es
            var mdQuery = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("Attributes", "PrimaryIdAttribute", "PrimaryNameAttribute"),
                Criteria = new MetadataFilterExpression
                {
                    Conditions =
                    {
                        new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals,
                            record.LogicalName)
                    }
                },
                AttributeQuery = new AttributeQueryExpression
                {
                    Properties = new MetadataPropertiesExpression("IsValidForCreate", "LogicalName")
                }
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(new RetrieveMetadataChangesRequest
            {
                Query = mdQuery,
                ClientVersionStamp = null
            });

            var cloned = new Entity(record.LogicalName);

            // Remplissage des attributs
            foreach (var attribute in response.EntityMetadata.First().Attributes)
            {
                if (toRemove.Contains(attribute.LogicalName))
                {
                    continue;
                }

                if (attribute.LogicalName == response.EntityMetadata.First().PrimaryIdAttribute)
                {
                    continue;
                }

                if (attribute.LogicalName == response.EntityMetadata.First().PrimaryNameAttribute)
                {
                    cloned[attribute.LogicalName] = $"{record.GetAttributeValue<string>(attribute.LogicalName)}{(addCloneLabel ? " (clone)" : "")}";
                    continue;
                }

                if ((attribute.IsValidForCreate ?? false) && record.Contains(attribute.LogicalName))
                {
                    if (record[attribute.LogicalName] is Guid)
                    {
                        continue;
                    }

                    cloned[attribute.LogicalName] = record[attribute.LogicalName];
                }
            }

            return cloned;
        }

        public static void CopyMultiSelectToTextField(this Entity entity, string sourceAttribute,
                    string targetAttribute, IOrganizationService service)
        {
            var collec = entity.GetAttributeValue<OptionSetValueCollection>(sourceAttribute);
            if (collec == null)
            {
                entity[targetAttribute] = null;
                return;
            }

            var amd = ((RetrieveAttributeResponse)service.Execute(new RetrieveAttributeRequest
            {
                EntityLogicalName = entity.LogicalName,
                LogicalName = sourceAttribute
            })).AttributeMetadata as MultiSelectPicklistAttributeMetadata;

            if (amd == null)
            {
                throw new Exception(
                    $"Impossible de trouver un attribut {sourceAttribute} dans l'entit� {entity.LogicalName}");
            }

            var text = string.Empty;

            foreach (var item in collec)
            {
                var option = amd.OptionSet?.Options?.FirstOrDefault(o => o.Value == item.Value);
                if (option != null)
                {
                    text += ", " + option.Label.UserLocalizedLabel.Label;
                }
            }

            entity[targetAttribute] = text.Length > 2 ? text.Remove(0, 2) : text;
        }

        public static void Delete(this Entity record, IOrganizationService service)
        {
            service.Delete(record.LogicalName, record.Id);
        }

        public static string ExtractAttributes(this Entity entity, Entity preimage)
        {
            var attrs = new StringBuilder();
            var keys = entity.Attributes.Keys
                .Where(k => k != "createdon" &&
                            k != "createdby" &&
                            k != "createdonbehalfby" &&
                            k != "modifiedon" &&
                            k != "modifiedby" &&
                            k != "modifiedonbehalfby" &&
                            k != entity.LogicalName + "id" &&
                            k != "activityid").ToList();
            keys.Sort();
            if (entity.Contains(entity.LogicalName + "id"))
            {
                keys.Insert(0, entity.LogicalName + "id");
            }
            if (entity.Contains("activityid"))
            {
                keys.Insert(0, "activityid");
            }
            var attlen = GetMaxAttributeNameLength(keys);
            foreach (string attr in keys)
            {
                object origValue, preValue, baseValue = null;
                string origType = string.Empty, resultValue;

                origValue = entity.Attributes[attr];

                if (origValue != null)
                {
                    origType = origValue.GetType().ToString();
                    if (origType.Contains("."))
                    {
                        origType = origType.Split('.')[origType.Split('.').Length - 1];
                    }
                    baseValue = AttributeToBaseType(origValue);
                }

                if (baseValue == null)
                {
                    resultValue = "<null>";
                }
                else
                {
                    resultValue = ValueToString(baseValue, attlen);
                    if (origValue is EntityReference)
                    {
                        var er = (EntityReference)origValue;
                        var erName = "No LogicalName available";
                        if (!string.IsNullOrEmpty(er.LogicalName))
                        {
                            erName = er.LogicalName;
                        }
                        if (!string.IsNullOrEmpty(er.Name))
                        {
                            erName += " " + er.Name;
                        }
                        resultValue += $" ({origType} {erName.Trim()})";
                    }
                    else
                    {
                        resultValue += " (" + origType + ")";
                    }
                }
                var newline = $"\n  {attr.PadRight(attlen)} = {resultValue}";
                attrs.Append(newline);
                if (preimage != null && !attr.Equals(entity.LogicalName + "id") && !attr.Equals("activityid") && preimage.Contains(attr))
                {
                    preValue = AttributeToBaseType(preimage[attr]);
                    if (preValue.Equals(baseValue))
                    {
                        preValue = "<not changed>";
                    }
                    attrs.Append($"\n   {("PRE").PadLeft(attlen)}: {ValueToString(preValue, attlen)}");
                }
            }
            return "  " + attrs.ToString().Trim();
        }

        /// <summary>
        /// Get value of an attribute from a linked entity
        /// </summary>
        /// <typeparam name="T">Type of the attribute to get</typeparam>
        /// <param name="record">Record containing the attribute</param>
        /// <param name="alias">Alias of the linked entity</param>
        /// <param name="attribute">Logical name of the attribute</param>
        /// <returns>Value of the attribute</returns>
        public static T GetAliasedValue<T>(this Entity record, string alias, string attribute)
        {
            var aliasedAttribute = $"{alias}.{attribute}";

            if (!record.Contains(aliasedAttribute))
            {
                return default(T);
            }

            var aliasedValue = record.GetAttributeValue<AliasedValue>(aliasedAttribute);

            if (!(aliasedValue.Value is T))
            {
                var message = $"The attribute {aliasedValue.AttributeLogicalName} in entity {aliasedValue.EntityLogicalName} is not of type {typeof(T)}";
                throw new InvalidCastException(message);
            }

            return (T)aliasedValue.Value;
        }

        public static string GetFormattedValue(this Entity record, string attribute)
        {
            if (!record.FormattedValues.Contains(attribute))
            {
                return string.Empty;
            }

            return record.FormattedValues[attribute];
        }

        public static T ToEntity<T>(this EntityReference reference) where T : Entity
        {
            return new Entity(reference.LogicalName, reference.Id).ToEntity<T>();
        }

        public static void Update(this Entity record, IOrganizationService service)
        {
            service.Update(record);
        }

        private static object AttributeToBaseType(object attribute)
        {
            if (attribute is AliasedValue)
                return AttributeToBaseType(((AliasedValue)attribute).Value);
            else if (attribute is EntityReference)
                return ((EntityReference)attribute).Id;
            else if (attribute is OptionSetValue)
                return ((OptionSetValue)attribute).Value;
            else if (attribute is Money)
                return ((Money)attribute).Value;
            else
                return attribute;
        }

        private static int GetMaxAttributeNameLength(List<string> keys)
        {
            var attlen = 0;
            foreach (string attr in keys)
            {
                attlen = Math.Max(attlen, attr.Length);
            }
            return attlen;
        }

        private static string ValueToString(object value, int baseIndent)
        {
            string resultValue = value.ToString();
            if (resultValue.Contains("\n"))
            {
                var newLinePad = new string(' ', baseIndent + 5);
                resultValue = resultValue.Replace("\n", "\n" + newLinePad);
            }

            return resultValue;
        }
    }
}