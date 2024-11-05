using Kev.Xrm.EntityWrappers;
using Kev.Xrm.Plugins.AppCode;
using Kev.Xrm.Plugins.AppCode.Attributes;
using Kev.Xrm.Service;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Kev.Xrm.Plugins
{
    [PrimaryEntity("contact")]
    public class ContactPlugin : Plugin
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
        public ContactPlugin(string unsecureConfiguration, string secureConfiguration)
        : base(unsecureConfiguration, secureConfiguration)
        {
        }


        [Description("Evénement Post Create sur le compte")]
        [Configuration("")]
        [SecureConfiguration("")]
        [SecondaryEntity("none")]
        [Mode(PluginMode.Synchronous)]
        [InvocationSource(InvocationSource.Parent)]
        [SupportedDeployment(SupportedDeployment.ServerOnly)]
        [Rank(1)]
        public override void PostOperationCreate(CreationServiceProvider csp)
        {
            var inputData = csp.Target.ToEntity<Contact>();
            csp.Trace($"Ce plugin s'est déclenché pour l'entité {inputData.LogicalName}");

            // Initialize ContactService (if needed for other actions later)
            ContactService contactService = new ContactService(csp.AdminService, csp.UserService, csp.TracingService);

            // Step 1: Check the user's division (which is an OptionSetValue, not a string)
            SystemUser user = (SystemUser)csp.UserService.Retrieve(SystemUser.EntityLogicalName, csp.Context.UserId, new ColumnSet(SystemUser.Fields.kev_DivisionCode));
            if (user != null && user.kev_DivisionCode != null && user.kev_DivisionCode.Value != 843000000)
            {
                throw new InvalidPluginExecutionException("Erreur: L'utilisateur doit appartenir à la division X pour créer un compte.");
            }

            csp.Trace("L'utilisateur appartient à la division X.");

            // Step 2: Assign the contact to the default title of the division
            contactService.UpdateContactFromUser(inputData, user.ToEntityReference());
            csp.Trace("Contact assigned to the appropriate division team.");
            csp.Trace("Traitement terminé");
        }


        [Description("Evénement Post Update sur le compte")]
        [Configuration("")]
        [SecureConfiguration("")]
        [SecondaryEntity("none")]
        [Mode(PluginMode.Synchronous)]
        [FilteringAttributes("parentcustomerid")]
        [InvocationSource(InvocationSource.Parent)]
        [SupportedDeployment(SupportedDeployment.ServerOnly)]
        [Rank(1)]
        public override void PostOperationUpdate(UpdateServiceProvider usp)
        {
            if (usp.Context.Depth > 1)
            {
                usp.Trace("Plugin execution skipped to prevent infinite loop.");
                return;
            }

            var inputData = usp.Target.ToEntity<Contact>();

            usp.Trace($"Plugin triggered for entity {inputData.LogicalName}");

            ContactService contactService = new ContactService(usp.AdminService, usp.UserService, usp.TracingService);


            // Handle change in parentcustomerid
            if (inputData.Contains(Contact.Fields.ParentCustomerId))
            {
                EntityReference newParentCustomerIdRef = inputData.GetAttributeValue<EntityReference>(Contact.Fields.ParentCustomerId);

                if (newParentCustomerIdRef == null || newParentCustomerIdRef.Id == Guid.Empty)
                {
                    usp.Trace("parentcustomerid is empty, skipping update.");
                }
                else
                {
                    usp.Trace($"Parentcustomerid updated for contact. Account ID: {newParentCustomerIdRef.Id}");
                    contactService.UpdateContactFromAccount(inputData, newParentCustomerIdRef);
                }
            }
        }


        public override void PreOperationUpdate(UpdateServiceProvider usp)
        {
            usp.Trace("Entering PreOperationUpdate.");

            // Check the depth to prevent infinite loops
            if (usp.Context.Depth > 1)
            {
                usp.Trace("Skipping plugin execution to avoid infinite loop.");
                return;
            }

            // Get the target entity
            var inputData = usp.Target.ToEntity<Contact>();
            var preImage = usp.GetPreImage<Contact>("PreImage");

            // Check if a Pre-Image exists (this contains the old values before the update)
            try
            {
                // Initialize the ContactService for further actions
                ContactService contactService = new ContactService(usp.AdminService, usp.UserService, usp.TracingService);

                // Step 1: Log history details before updating the contact
                contactService.AddContactHistory(inputData, preImage);

                usp.Trace("Pre-Operation Update processing completed.");
            }
            catch (Exception ex)
            {
                usp.Trace($"Error occurred: {ex.Message}");
            }
        }
    }
}