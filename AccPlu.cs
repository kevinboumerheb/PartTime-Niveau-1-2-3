using Kevs.Xrm.EntityWrappers;
using Kevs.Xrm.Plugins.AppCode;
using Kevs.Xrm.Plugins.AppCode.Attributes;
using Kevs.Xrm.Service;
using Microsoft.Xrm.Sdk;

namespace Kevs.Xrm.Plugins
{
    [PrimaryEntity(Account.EntityLogicalName)]
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

        public override void PreOperationCreate(ExtendedServiceProvider esp)
        {
            Account account = esp.GetInputData<Account>();
            AccountService accountService = new AccountService(esp.AdminService, esp.UserService, esp.TracingService);
            accountService.UpdateAccountNumber(account);
        }

        // Plugin #101 & #104
        public override void PreOperationUpdate(ExtendedServiceProvider esp)
        {
            Account newAccount = esp.GetInputData<Account>();
            Account oldAccount = esp.GetPreImage<Account>("PreImage");
            AccountService accountService = new AccountService(esp.AdminService, esp.UserService, esp.TracingService);
            accountService.UpdateAccountInfoFromContact(newAccount);
            accountService.UpdateGrowthOfTheAccount(newAccount, oldAccount);
        }


    }
}
