using Kev.Xrm.EntityWrappers;
using Kev.Xrm.Service.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Kev.Xrm.Service
{
    public class AccountService : ServiceBase
    {
        #region constructeur

        public AccountService(IOrganizationService adminService,
            IOrganizationService userService,
            ITracingService tracingService = null)
            : base(adminService, userService, tracingService)
        {
        }

        #endregion constructeur

        public void UpdateCroissanceField(Account account, int oldEmployeeCount, int newEmployeeCount)
        {
            TraceMethodStart();

            if (newEmployeeCount > oldEmployeeCount)
            {
                account.kev_GrowthCode = new OptionSetValue(843000000);  // Positive growth
                Trace("La croissance a été mise à jour à Positive.");
            }
            else
            {
                account.kev_GrowthCode = new OptionSetValue(843000001);  // Negative growth
                Trace("La croissance a été mise à jour à Négative.");
            }

            // Make sure the update does not trigger the plugin again
            UserService.Update(account);

            Trace("Compte mis à jour avec succès.");

            TraceMethodEnd();
        }


        public void UpdateAccountFromContact(Account account, EntityReference contactRef)
        {
            TraceMethodStart();

            // Retrieve contact information using early-bound Contact entity
            Contact contact = UserService.Retrieve(Contact.EntityLogicalName, contactRef.Id, new ColumnSet("emailaddress1", "address1_telephone1", "address1_addresstypecode", "preferredsystemuserid", "birthdate")).ToEntity<Contact>();

            if (contact != null)
            {
                // Update the account with contact details
                if (contact.EMailAddress1 != null)
                    account.EMailAddress1 = contact.EMailAddress1;
                if (contact.Address1_Telephone1 != null)
                    account.Telephone1 = contact.Address1_Telephone1;
                if (contact.Address1_AddressTypeCode != null)
                    account.Address1_AddressTypeCode = contact.Address1_AddressTypeCode;
                if (contact.PreferredSystemUserId != null)
                    account.PreferredSystemUserId = contact.PreferredSystemUserId;
                if (contact.BirthDate != null)
                    account.kev_Birthday = contact.BirthDate;

                // Update the account
                UserService.Update(account);
                Trace("Account updated with contact details.");
            }
            else
            {
                Trace("Contact not found or does not contain expected data.");
            }

            TraceMethodEnd();
        }
    }
}
