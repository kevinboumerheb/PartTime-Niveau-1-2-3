using Kev.Xrm.EntityWrappers;
using Kev.Xrm.Service.Base;
using Kev.Xrm.Utilities.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Kev.Xrm.Utilities;
using System.Diagnostics;

namespace Kev.Xrm.Service
{
    public class ContactService : ServiceBase
    {
        #region constructeur

        public ContactService(IOrganizationService adminService,
            IOrganizationService userService,
            ITracingService tracingService = null)
            : base(adminService, userService, tracingService)
        {
        }

        #endregion constructeur

        public void UpdateContactFromUser(Contact contact, EntityReference userRef)
        {
            TraceMethodStart();

            // Retrieve user information (early-bound)
            SystemUser user = (SystemUser)UserService.Retrieve(SystemUser.EntityLogicalName, userRef.Id, new ColumnSet(SystemUser.Fields.JobTitle));
            if (user != null)
            {
                // Update the contact with user details
                if (user.JobTitle != null)
                    contact.JobTitle = user.JobTitle;

                // Update the account
                UserService.Update(contact);
                Trace("Contact updated with user details.");
            }
            else
            {
                Trace("User not found or does not contain expected data.");
            }

            TraceMethodEnd();
        }

        public void UpdateContactFromAccount(Contact contact, EntityReference accountRef)
        {
            TraceMethodStart();

            // Retrieve account information (early-bound)
            Account account = (Account)UserService.Retrieve(Account.EntityLogicalName, accountRef.Id, new ColumnSet(Account.Fields.WebSiteURL));
            if (account != null)
            {
                // Update the contact with account details
                if (account.WebSiteURL != null)
                    contact.WebSiteUrl = account.WebSiteURL;

                // Update the account
                UserService.Update(contact);
                Trace("Contact updated with account details.");
            }
            else
            {
                Trace("Account not found or does not contain expected data.");
            }

            TraceMethodEnd();
        }

        public void AddContactHistory(Contact contact, Contact preImage)
        {
            TraceMethodStart();

            try
            {
                // Step 1: Create a new ContactHistory entity
                var contactVersion = new kev_ContactVersion();

                // Copy data from PreImage (the old contact details before the update)
                contactVersion.kev_FirstName = preImage.FirstName;
                contactVersion.kev_LastName = preImage.LastName;
                contactVersion.kev_AccountId = preImage.ParentCustomerId;
                contactVersion.kev_Email = preImage.EMailAddress1;
                contactVersion.kev_Telephone = preImage.Address1_Telephone1;
                contactVersion.kev_AddressTypeCode = preImage.Address1_AddressTypeCode;
                contactVersion.kev_PreferredUserId = preImage.PreferredSystemUserId;
                contactVersion.kev_Birthday = preImage.BirthDate;
                contactVersion.kev_JobTitle = preImage.JobTitle;
                contactVersion.kev_Website = preImage.WebSiteUrl;

                // Associate the history record with the original Contact record
                contactVersion.kev_ContactId = new EntityReference(Contact.EntityLogicalName, preImage.Id);

                // Save the ContactHistory record
                AdminService.Create(contactVersion);

                Trace("ContactHistory record created successfully.");
                // Step 2: Create a new Contact from pre-image

                Entity clonedContact = preImage.Clone(
                    AdminService,
                    true,
                    "contactid"  // Remove the GUID field
                );


                // Create the cloned contact and retrieve the new ID
                Guid newContactId = AdminService.Create(clonedContact);

                // Explicitly set the ID for the cloned contact
                clonedContact.Id = newContactId;

                Contact previousVersion = clonedContact.ToEntity<Contact>();
                previousVersion.Id = newContactId; // Explicitly set the ID here


                Trace("New Contact created successfully.");

                // Step 3: Update the original Contact's "previous" lookup field to reference the new Contact
                contact.kev_PreviousVersionId = new EntityReference(Contact.EntityLogicalName, newContactId);

                // Update the current Contact
                AdminService.Update(contact);

                Trace("Updated current contact with reference to the new contact in 'previous' lookup field.");

                CloneManyToManyRelationships(preImage, previousVersion);
                LogClonedRelationships(previousVersion);

                TraceMethodEnd();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Error cloning contact: {ex.Message}");
            }

            TraceMethodEnd();
        }

        private void LogClonedRelationships(Contact clone)
        {
            QueryExpression query = new QueryExpression("adx_invitation")
            {
                ColumnSet = new ColumnSet("adx_invitationid")
            };
            query.Criteria.AddCondition(new ConditionExpression("adx_invitecontact", ConditionOperator.Equal, clone.Id));

            EntityCollection relatedEntities = AdminService.RetrieveMultiple(query);
            Trace($"Cloned contact {clone.Id} has {relatedEntities.Entities.Count} related 'adx_invitation' records.");

            foreach (Entity relatedEntity in relatedEntities.Entities)
            {
                Trace($"Related adx_invitation ID: {relatedEntity.Id}");
            }
        }

        private void CloneManyToManyRelationships(Contact original, Contact clone)
        {
            //var relationshipName = "adx_invitation_invitecontacts";

            Trace($"Starting clone of many-to-many relationships for original contact ID: {original.Id} to clone ID: {clone.Id}.");

            QueryExpression query = new QueryExpression("adx_invitation")
            {
                ColumnSet = new ColumnSet(true) // Retrieves all fields to ensure full cloning
            };
            query.Criteria.AddCondition(new ConditionExpression("adx_invitecontact", ConditionOperator.Equal, original.Id));

            Trace("Executing RetrieveMultiple query to find related entities.");
            EntityCollection relatedEntities = AdminService.RetrieveMultiple(query);

            Trace($"Number of related entities retrieved: {relatedEntities.Entities.Count}");

            foreach (Entity relatedEntity in relatedEntities.Entities)
            {
                Trace($"Cloning related entity with ID: {relatedEntity.Id}");

                // Use the Clone method to create a copy of the related entity, removing the GUID field
                Entity clonedRelatedEntity = relatedEntity.Clone(
                    AdminService,
                    true,
                    "adx_invitationid" // Remove the GUID field to ensure a new entity is created
                );

                // Associate the cloned entity with the clone contact
                clonedRelatedEntity["adx_invitecontact"] = new EntityReference("contact", clone.Id);

                Trace($"Creating cloned related entity with new reference to clone ID: {clone.Id}");
                AdminService.Create(clonedRelatedEntity);
            }

            Trace("Many-to-many relationships cloned successfully.");
        }

        // Retrieve the list of contacts
        public List<Contact> RetrieveContacts(CrmServiceClient service)
        {
            // Define the query to retrieve contacts
            QueryExpression query = new QueryExpression(Contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Contact.Fields.ContactId, Contact.Fields.FullName, Contact.Fields.StateCode, Contact.Fields.BirthDate, Contact.Fields.AccountRoleCode, Contact.Fields.Address1_City),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression(Contact.Fields.OwnerId, ConditionOperator.NotEqual, new Guid("ec04d6ee-b830-4759-8267-00e354b3c152"))
                        }
                }
            };

            // Execute the query
            EntityCollection contacts = UserService.RetrieveMultiple(query);

            // Convert the EntityCollection to a list of Contact objects
            List<Contact> contactList = contacts.Entities.Select(e => e.ToEntity<Contact>()).ToList();

            return contactList;
        }

        // Method to display contact types
        public void DisplayContactTypes(CrmServiceClient service, List<Contact> contacts, string logFilePath)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = Contact.EntityLogicalName,
                LogicalName = Contact.Fields.AccountRoleCode,
                RetrieveAsIfPublished = true
            };

            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
            PicklistAttributeMetadata metadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;

            foreach (var option in metadata.OptionSet.Options)
            {
                int count = contacts.Count(c => c.AccountRoleCode != null && c.AccountRoleCode.Value == option.Value);
                LogAndDisplayHelper.LogAndDisplay($"{option.Label.UserLocalizedLabel.Label}: {count}", logFilePath);
            }
        }

        // Helper method to update contacts with city lookup
        public void UpdateContactsWithCityLookup(CrmServiceClient service, List<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                // Simulate city lookup assignment
                string cityName = contact.Address1_City;
                if (!string.IsNullOrEmpty(cityName))
                {
                    kev_Ville city = FindOrCreateCity(service, cityName);
                    if (city != null)
                    {
                        contact.kev_VilleId = new EntityReference(kev_Ville.EntityLogicalName, city.Id);
                        service.Update(contact);
                    }
                }
            }
        }

        // Helper method to find or create a city
        private kev_Ville FindOrCreateCity(CrmServiceClient service, string cityName)
        {
            QueryExpression cityQuery = new QueryExpression(kev_Ville.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(kev_Ville.Fields.kev_VilleId),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(kev_Ville.Fields.kev_Name, ConditionOperator.Equal, cityName)
                    }
                }
            };
            EntityCollection cities = service.RetrieveMultiple(cityQuery);
            if (cities.Entities.Any())
                return cities.Entities.First().ToEntity<kev_Ville>();

            // Create a new city if it does not exist
            kev_Ville newCity = new kev_Ville { kev_Name = cityName };
            newCity.Id = service.Create(newCity);
            return newCity;
        }

        // Helper method to update contacts with city lookup using ExecuteMultipleRequest
        public void UpdateContactsWithCityLookupUsingExecuteMultiple(CrmServiceClient service, List<Contact> contacts, string logFilePath)
        {
            // Create an ExecuteMultipleRequest object
            var multipleRequest = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true, // Continue processing if one request fails
                    ReturnResponses = true  // Get responses for each individual request
                }
            };

            foreach (var contact in contacts)
            {
                // Simulate city lookup assignment
                string cityName = contact.Address1_City;
                if (!string.IsNullOrEmpty(cityName))
                {
                    kev_Ville city = FindOrCreateCity(service, cityName);
                    if (city != null)
                    {
                        contact.kev_VilleId = new EntityReference(kev_Ville.EntityLogicalName, city.Id);

                        // Instead of calling service.Update(contact), create an UpdateRequest
                        var updateRequest = new UpdateRequest { Target = contact };

                        // Add each update request to the batch
                        multipleRequest.Requests.Add(updateRequest);
                    }
                }
            }

            // Execute the batch request if there are requests to process
            if (multipleRequest.Requests.Count > 0)
            {
                var response = (ExecuteMultipleResponse)service.Execute(multipleRequest);

                // Process the responses
                foreach (var responseItem in response.Responses)
                {
                    if (responseItem.Fault != null)
                    {
                        LogAndDisplayHelper.LogAndDisplay($"Error updating contact: {responseItem.Fault.Message}", logFilePath);
                    }
                }
            }
        }

        private static Random random = new Random();

        // Helper method to generate a TVA Number with the format "FR0000"
        private string GenerateTVANumber()
        {
            int randomNumber = random.Next(0, 1000);
            return $"FR{randomNumber:D4}"; // Format as "FR0000" up to "FR1000"
        }

        // Method to create 1000 contacts without ExecuteMultipleRequest
        private void CreateContactsIndividually(CrmServiceClient service)
        {
            for (int i = 0; i < 10; i++)
            {
                Contact contact = new Contact
                {
                    FirstName = $"FirstName_{i}",
                    LastName = $"LastName_{i}",
                    kev_TVANumber = GenerateTVANumber()
                };

                service.Create(contact); // Individual create request
            }
        }

        // Method to create 1000 contacts with ExecuteMultipleRequest
        private void CreateContactsWithExecuteMultiple(CrmServiceClient service, string logFilePath)
        {
            var multipleRequest = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                }
            };

            //for (int i = 0; i < 1000; i++)
            //{
            //    Contact contact = new Contact
            //    {
            //        FirstName = $"FirstName_{i}_WithExecuteMultiple",
            //        LastName = $"LastName_{i}",
            //        kev_TVANumber = GenerateTVANumber()
            //    };
            //    var createRequest = new CreateRequest { Target = contact };
            //    multipleRequest.Requests.Add(createRequest);

            //    // Execute in batches of 100 to manage payload
            //    if (multipleRequest.Requests.Count == 100 || i == 1999)
            //    {
            //        var response = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            //        foreach (var responseItem in response.Responses)
            //        {
            //            if (responseItem.Fault != null)
            //            {
            //                LogAndDisplayHelper.LogAndDisplay($"Error creating contact: {responseItem.Fault.Message}", logFilePath);
            //            }
            //        }

            //        multipleRequest.Requests.Clear(); // Clear requests for the next batch
            //    }
            //}

            for (int i = 0; i < 10; i++)
            {
                Entity contact = new Entity("contact");
                contact["firstname"] = $"FirstName_{i}_WithExecuteMultiple";
                contact["lastname"] = $"LastName_{i}";
                contact["kev_tvanumber"] = GenerateTVANumber();

                var createRequest = new CreateRequest { Target = contact };
                multipleRequest.Requests.Add(createRequest);
            }

            // Execute the batch request for the 10 contacts
            var response = (ExecuteMultipleResponse)service.Execute(multipleRequest);

            foreach (var responseItem in response.Responses)
            {
                if (responseItem.Fault != null)
                {
                    LogAndDisplayHelper.LogAndDisplay($"Error creating contact: {responseItem.Fault.Message}", logFilePath);
                }
            }
        }

        // Method to run both approaches and measure execution time
        public void RunComparison(CrmServiceClient service, string logFilePath)
        {
            // Measure time for individual creation
            var stopwatch = Stopwatch.StartNew();
            CreateContactsIndividually(service);
            stopwatch.Stop();
            LogAndDisplayHelper.LogAndDisplay($"Time taken without ExecuteMultiple: {stopwatch.Elapsed}", logFilePath);

            // Measure time for batched creation using ExecuteMultiple
            stopwatch.Restart();
            CreateContactsWithExecuteMultiple(service, logFilePath);
            stopwatch.Stop();
            LogAndDisplayHelper.LogAndDisplay($"Time taken with ExecuteMultiple: {stopwatch.Elapsed}", logFilePath);
        }
    }
}