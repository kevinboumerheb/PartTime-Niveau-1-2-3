using Kevs.Xrm.EntityWrappers;
using Kevs.Xrm.Service.Base;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Kevs.Xrm.Service
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

        public void UpdateAccountNumber( Account account /*for console app Entity enaccount*/)
        {
            /*// For Console App
            Account account= enaccount.ToEntity<Account>();*/

            //QueryExpression query = new QueryExpression(kev_Configuration.EntityLogicalName)
            //{
            //    ColumnSet = new ColumnSet(kev_Configuration.Fields.kev_Value),
            //    Criteria =
            //    {
            //        Conditions =
            //        {
            //            new ConditionExpression(kev_Configuration.Fields.kev_Name,ConditionOperator.Equal,"AccountNumber")
            //        }
            //    },
            //    TopCount=1
            //};


            //EntityCollection result = AdminService.RetrieveMultiple(query);

            //kev_Configuration configuration = result.Entities.FirstOrDefault()?.ToEntity<kev_Configuration>();


            kev_Configuration configuration = AdminService.Retrieve(
                kev_Configuration.EntityLogicalName,
                new Guid("85f8a410-d287-f011-b4cb-6045bd1a70a9"), 
                new ColumnSet(
                        kev_Configuration.Fields.kev_Value
                    )
                ).ToEntity<kev_Configuration>();


            int accountCount = int.Parse( configuration.kev_Value);

            Trace($"Total accounts found: {accountCount}");
            account.AccountNumber = (accountCount + 1).ToString();

            kev_Configuration updatedConfiguration = new kev_Configuration
            {
                kev_Value = account.AccountNumber,
                Id = configuration.Id
            };
            AdminService.Update(updatedConfiguration);
            
        }

        // Plugin #101
        public void UpdateAccountInfoFromContact(Account account)
        {
            if (account.Contains(Account.Fields.PrimaryContactId))
            {
                TraceMethodStart();

                Contact contact = AdminService.Retrieve(
                    Contact.EntityLogicalName,
                    account.PrimaryContactId.Id,
                    new ColumnSet
                        (
                            Contact.Fields.EMailAddress1,
                            Contact.Fields.Telephone1,
                            Contact.Fields.BirthDate,
                            Contact.Fields.PreferredSystemUserId,
                            Contact.Fields.Address1_AddressTypeCode
                        )
                ).ToEntity<Contact>();

                account.EMailAddress1 = contact.EMailAddress1;
                account.Fax = contact.Telephone1;
                account.LastUsedInCampaign = contact.BirthDate;
                account.PreferredSystemUserId = contact.PreferredSystemUserId;
                account.Address1_AddressTypeCode = contact.Address1_AddressTypeCode;
                TraceMethodEnd();
            }
        }

        // Plugin #104
        public void UpdateGrowthOfTheAccount(Account newAccount, Account oldaccount)
        {
            if (newAccount.Contains(Account.Fields.NumberOfEmployees) && oldaccount.Contains(Account.Fields.NumberOfEmployees))
            {
                TraceMethodStart();
                if(newAccount.NumberOfEmployees > oldaccount.NumberOfEmployees)
                {
                    newAccount.kev_GrowthCode = new OptionSetValue(0);
                }
                else if (newAccount.NumberOfEmployees < oldaccount.NumberOfEmployees)
                {
                    newAccount.kev_GrowthCode = new OptionSetValue(1);
                }
                else
                {
                    newAccount.kev_GrowthCode = new OptionSetValue(2);
                }
                TraceMethodEnd();
            }
        }

    }
}
