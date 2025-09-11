using Kevs.Xrm.EntityWrappers;
using Kevs.Xrm.Service.Base;
using Kevs.Xrm.Utilities.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Security.Principal;

namespace Kevs.Xrm.Service
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

        // Plugin #102 - a
        public void ThrowIfContactNotInDivisionX(Guid userId)
        {
            if (userId != null)
            {
                TraceMethodStart();

                SystemUser currentUser = AdminService.Retrieve(
                    SystemUser.EntityLogicalName,
                    userId,
                    new ColumnSet
                        (
                            SystemUser.Fields.kev_DivisionCode
                        )
                ).ToEntity<SystemUser>();
                if (currentUser.kev_DivisionCode == null || currentUser.kev_DivisionCode.Value != 0)
                {
                    throw new InvalidPluginExecutionException($"You must be in Division X to create this contact (user {userId}).");
                }

                TraceMethodEnd();
            }
        }

        // Plugin #102 - b
        public void UpdateContactWithDivisionX(Contact contact, Guid userId)
        {   
            if (contact != null)
            {
                TraceMethodStart();
                SystemUser currentUser = AdminService.Retrieve(
                    SystemUser.EntityLogicalName,
                    userId,
                    new ColumnSet
                        (
                            SystemUser.Fields.kev_DivisionCode
                        )
                ).ToEntity<SystemUser>();

                contact.kev_DivisionCode = currentUser.kev_DivisionCode;
                TraceMethodEnd();
            }
        }

        // Plugin #103
        public void UpdateContactFromAccount(Contact contact)
        {
            if (contact.Contains(Contact.Fields.ParentCustomerId))
            {
                TraceMethodStart();

                Account account = AdminService.Retrieve(
                    Account.EntityLogicalName,
                    contact.ParentCustomerId.Id,
                    new ColumnSet
                        (
                            Account.Fields.EMailAddress1,
                            Account.Fields.Telephone1,
                            Account.Fields.WebSiteURL
                        )
                ).ToEntity<Account>();

                contact.EMailAddress1 = account.EMailAddress1;
                contact.Telephone1 = account.Telephone1;
                contact.WebSiteUrl = account.WebSiteURL;

                TraceMethodEnd();
            }
        }

        // Plugin #201
        public void CreateContactHistory(Contact contact, Contact preImage)
        {
            if (contact != null && preImage != null)
            {
                TraceMethodStart();

                Contact ClonedContact = preImage.Clone(AdminService, true, Contact.Fields.ContactId).ToEntity<Contact>();

                ClonedContact.Id = AdminService.Create(ClonedContact);
                //contact.kev_PreviousVersionId = ClonedContact.Id;?????
                contact.kev_PreviousVersionId = new EntityReference(Contact.EntityLogicalName, ClonedContact.Id);
                TraceMethodEnd();
            }
        }
    }
}
