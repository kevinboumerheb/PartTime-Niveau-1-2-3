using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Kev.Xrm.Utilities.Extensions
{
    public static class OrganizationServiceExtensions
    {
        public static void Associate(this IOrganizationService service, EntityReference reference, string relationshipName, EntityReferenceCollection records)
        {
            service.Associate(reference.LogicalName, reference.Id, new Relationship(relationshipName), records);
        }

        public static void Associate(this IOrganizationService service, EntityReference reference, string relationshipName, EntityReference record)
        {
            service.Associate(reference, relationshipName, new EntityReferenceCollection(new List<EntityReference> { record }));
        }

        /// <summary>
        /// Delete a record by its reference
        /// </summary>
        /// <param name="service">Organization Service</param>
        /// <param name="reference">Reference of the record to delete</param>
        public static void Delete(this IOrganizationService service, EntityReference reference)
        {
            service.Delete(reference.LogicalName, reference.Id);
        }

        /// <summary>
        /// Delete a record
        /// </summary>
        /// <param name="service">Organization Service</param>
        /// <param name="record">Record to delete</param>
        public static void Delete(this IOrganizationService service, Entity record)
        {
            service.Delete(record.LogicalName, record.Id);
        }

        public static List<string> GetCurrentUserRoles(this IOrganizationService service)
        {
            var query = new QueryExpression("role")
            {
                NoLock = true,
                ColumnSet = new ColumnSet("name"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "role",
                        LinkFromAttributeName = "roleid",
                        LinkToAttributeName = "roleid",
                        LinkToEntityName = "systemuserroles",
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = "systemuserroles",
                                LinkFromAttributeName = "systemuserid",
                                LinkToAttributeName = "systemuserid",
                                LinkToEntityName = "systemuser",
                                LinkCriteria = new FilterExpression
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return service.RetrieveMultiple(query).Entities.Select(e => e.GetAttributeValue<string>("name")).ToList();
        }

        public static CultureInfo GetUserCulture(this IOrganizationService service, Guid userId)
        {
            var userSettings = service.RetrieveSingle("usersettings", "systemuserid", userId, false, "uilanguageid");

            return CultureInfo.GetCultureInfo(userSettings.GetAttributeValue<int>("uilanguageid"));
        }

        /// <summary>
        /// Retrieves the first record of the query if exists
        /// </summary>
        /// <typeparam name="T">Type of the record to return</typeparam>
        /// <param name="service">Organization Service</param>
        /// <param name="query">Query</param>
        /// <returns>First record if found</returns>
        public static T RetrieveFirstOrDefault<T>(this IOrganizationService service, QueryBase query) where T : Entity
        {
            var result = service.RetrieveMultiple(query);

            return result.Entities.Select(e => e.ToEntity<T>()).FirstOrDefault();
        }

        public static EntityCollection RetrieveMultiple(this IOrganizationService service, string entityName, bool allColumns, params string[] cols)
        {
            return service.RetrieveMultiple(entityName, new string[0], new object[0], allColumns, cols);
        }

        public static EntityCollection RetrieveMultiple(this IOrganizationService service, string entityName, string attribute, object value, bool allColumns, params string[] cols)
        {
            return service.RetrieveMultiple(entityName, new[] { attribute }, new[] { value }, allColumns, cols);
        }

        public static EntityCollection RetrieveMultiple(this IOrganizationService service, string entityName, string[] attributes, object[] values, bool allColumns, params string[] cols)
        {
            var query = new QueryExpression(entityName)
            {
                NoLock = true,
                ColumnSet = allColumns ? new ColumnSet(true) : new ColumnSet(cols),
                Criteria = new FilterExpression(),
                PageInfo =
                {
                    Count = 5000,
                    PageNumber = 1
                }
            };

            for (int i = 0; i < attributes.Length; i++)
            {
                query.Criteria.AddCondition(attributes[i], ConditionOperator.Equal, values[i]);
            }

            EntityCollection all = new EntityCollection
            {
                EntityName = entityName,
            };
            EntityCollection partial;

            do
            {
                partial = service.RetrieveMultiple(query);

                all.Entities.AddRange(partial.Entities);

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = partial.PagingCookie;
            } while (partial.MoreRecords);

            return all;
        }

        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, string attribute, object value, bool allColumns, params string[] cols) where T : Entity
        {
            return service.RetrieveMultiple<T>(new[] { attribute }, new[] { value }, allColumns, cols);
        }

        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, string[] attributes, object[] values, bool allColumns, params string[] cols) where T : Entity
        {
            string entityName = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            var query = new QueryExpression(entityName)
            {
                NoLock = true,
                ColumnSet = allColumns ? new ColumnSet(true) : new ColumnSet(cols),
                Criteria = new FilterExpression(),
                PageInfo =
                {
                    Count = 5000,
                    PageNumber = 1
                }
            };

            for (int i = 0; i < attributes.Length; i++)
            {
                query.Criteria.AddCondition(attributes[i], ConditionOperator.Equal, values[i]);
            }

            EntityCollection all = new EntityCollection
            {
                EntityName = entityName
            };
            EntityCollection partial;

            do
            {
                partial = service.RetrieveMultiple(query);

                all.Entities.AddRange(partial.Entities);

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = partial.PagingCookie;
            } while (partial.MoreRecords);

            return all.ToEntities<T>();
        }

        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, QueryBase query) where T : Entity
        {
            return service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<T>()).ToList();
        }

        public static List<T> RetrieveMultiple<T>(this IOrganizationService service, string logicalName, FilterExpression filter, params string[] cols) where T : Entity
        {
            int fetchCount = 250;
            int pageNumber = 1;
            QueryExpression query = new QueryExpression
            {
                EntityName = logicalName,
                ColumnSet = cols.Length == 0 ? new ColumnSet(true) : new ColumnSet(cols),
                Criteria = filter
            };
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;
            List<T> list = new List<T>();
            while (true)
            {
                EntityCollection results = service.RetrieveMultiple(query);
                if (results.Entities != null)
                {
                    foreach (Entity entity in results.Entities)
                    {
                        var clb = entity.ToEntity<T>();
                        list.Add(clb);
                    }
                }
                if (results.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    break;
                }
            }
            return list.ToList();
        }

        public static Entity RetrieveSingle(this IOrganizationService service, EntityReference reference, bool allColumns, params string[] cols)
        {
            return service.RetrieveSingle(reference.LogicalName, reference.LogicalName + "id", reference.Id, allColumns, cols);
        }

        public static Entity RetrieveSingle(this IOrganizationService service, string entityName, Guid id, bool allColumns, params string[] cols)
        {
            return service.RetrieveSingle(entityName, entityName + "id", id, allColumns, cols);
        }

        public static Entity RetrieveSingle(this IOrganizationService service, string entityName, string attribute, object value, bool allColumns, params string[] cols)
        {
            return service.RetrieveSingle(entityName, new[] { attribute }, new[] { value }, allColumns, cols);
        }

        public static Entity RetrieveSingle(this IOrganizationService service, string entityName, string[] attributes, object[] values, bool allColumns, params string[] cols)
        {
            var query = new QueryExpression(entityName)
            {
                TopCount = 1,
                NoLock = true,
                ColumnSet = allColumns ? new ColumnSet(true) : new ColumnSet(cols),
                Criteria = new FilterExpression()
            };

            for (int i = 0; i < attributes.Length; i++)
            {
                query.Criteria.AddCondition(attributes[i], ConditionOperator.Equal, values[i]);
            }

            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, EntityReference reference, bool allColumns, params string[] cols) where T : Entity
        {
            return service.RetrieveSingle<T>(reference.LogicalName, reference.LogicalName + "id", reference.Id, allColumns, cols);
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, Guid id, bool allColumns, params string[] cols) where T : Entity
        {
            string entityName = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();
            return service.RetrieveSingle<T>(entityName, entityName + "id", id, allColumns, cols);
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, string entityName, Guid id, bool allColumns, params string[] cols) where T : Entity
        {
            return service.RetrieveSingle<T>(entityName, entityName + "id", id, allColumns, cols);
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, string entityName, string attribute, object value, bool allColumns, params string[] cols) where T : Entity
        {
            return service.RetrieveSingle<T>(entityName, new[] { attribute }, new[] { value }, allColumns, cols);
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, string[] attributes, object[] values, bool allColumns, params string[] cols) where T : Entity
        {
            string entityName = typeof(T).GetField("EntityLogicalName").GetRawConstantValue().ToString();

            return service.RetrieveSingle<T>(entityName, attributes, values, allColumns, cols);
        }

        public static T RetrieveSingle<T>(this IOrganizationService service, string entityName, string[] attributes, object[] values, bool allColumns, params string[] cols) where T : Entity
        {
            var query = new QueryExpression(entityName)
            {
                TopCount = 1,
                NoLock = true,
                ColumnSet = cols.Length == 0 ? new ColumnSet(true) : new ColumnSet(cols),
                Criteria = new FilterExpression()
            };

            for (int i = 0; i < attributes.Length; i++)
            {
                query.Criteria.AddCondition(attributes[i], ConditionOperator.Equal, values[i]);
            }

            var record = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (record == null)
            {
                return default(T);
            }

            return record.ToEntity<T>();
        }

        /// <summary>
        /// Create or Update a record, depending if it already exists in the targeted organization
        /// </summary>
        /// <param name="service">Organization Service</param>
        /// <param name="record">Record to process</param>
        /// <returns>A value that indicates if the record was created (true) or updated (false)</returns>
        public static bool Upsert(this IOrganizationService service, Entity record)
        {
            var request = new UpsertRequest
            {
                Target = record
            };

            var response = (UpsertResponse)service.Execute(request);

            return response.RecordCreated;
        }
    }
}