using Kev.Xrm.EntityWrappers;
using Kev.Xrm.Plugins.AppCode;
using Kev.Xrm.Plugins.AppCode.Attributes;
using Kev.Xrm.Service;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Kev.Xrm.Plugins
{
    [PrimaryEntity("account")]
    public class AccountPlugin : Plugin
    {
        /// <summary>
        /// Le constructeur est nécessaire pour passer les configurations
        /// de fonctionnalité
        /// </summary>
        /// <remarks>
        /// La configuration de fonctionnalité doit être définie dans la
        /// configuration non sécurisée de l'étape de traitement de plugin
        /// comme ci-dessous:
        /// {"Features":[{"Enabled":true,"Name":"MaFonctionnalité"},{"Enabled":false,"Name":"feature2"}]}
        /// </remarks>
        /// <param name="unsecureConfiguration"></param>
        /// <param name="secureConfiguration"></param>
        public AccountPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
        {
        }

        [Description("Evénement Post Update sur le compte")]
        [Configuration("")]
        [SecureConfiguration("")]
        [SecondaryEntity("none")]
        [Mode(PluginMode.Synchronous)]
        [FilteringAttributes("numberofemployees, primarycontactid")]
        [InvocationSource(InvocationSource.Parent)]
        [SupportedDeployment(SupportedDeployment.ServerOnly)]
        [Image("PreImage", PluginImageType.Pre, "numberofemployees")]
        [Rank(1)]
        public override void PostOperationUpdate(UpdateServiceProvider usp)
        {
            if (usp.Context.Depth > 1)
            {
                usp.Trace("Plugin execution skipped to prevent infinite loop.");
                return;
            }

            // Early bound entity
            var inputData = usp.Target.ToEntity<Account>();

            usp.Trace($"Plugin triggered for entity {inputData.LogicalName}");

            AccountService accountService = new AccountService(usp.AdminService, usp.UserService, usp.TracingService);

            // Handle change in primary contact
            if (inputData.Contains("primarycontactid"))
            {
                var newContactRef = inputData.PrimaryContactId;

                if (newContactRef == null || newContactRef.Id == Guid.Empty)
                {
                    usp.Trace("Primary contact ID is empty, skipping update.");
                }
                else
                {
                    usp.Trace($"Primary contact updated for account. Contact ID: {newContactRef.Id}");
                    accountService.UpdateAccountFromContact(inputData, newContactRef);
                }

                return;
            }

            var preImage = usp.GetPreImage<Account>("PreImage");

            // Handle change in number of employees
            if (preImage != null && preImage.Contains("numberofemployees") && inputData.Contains("numberofemployees"))
            {
                int oldEmployeeCount = preImage.NumberOfEmployees ?? 0;
                int newEmployeeCount = inputData.NumberOfEmployees ?? 0;

                usp.Trace($"Old Number of Employees: {oldEmployeeCount}, New Number of Employees: {newEmployeeCount}");

                accountService.UpdateCroissanceField(inputData, oldEmployeeCount, newEmployeeCount);
            }
        }
    }
}
