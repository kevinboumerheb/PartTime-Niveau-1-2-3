using Kev.Xrm.Utilities;
using Kev.Xrm.Utilities.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Kev.Xrm.Plugins.AppCode
{
    /// <summary>
    /// Classe représentant les traitements réalisés par un plugin
    /// </summary>
    /// <remarks>
    ///
    /// Cette classe ne doit contenir aucun code métier. Elle sert uniquement à
    /// déclencher des événements en fonction d'étape de traitement et de message
    ///
    /// Pour ajouter la gestion d'un message particulier:
    /// - Compléter les switch/case des messages dans la méthode "Execute"
    /// - Ajouter une méthode virtuelle dans la région "Méthodes virtuelles"
    /// - Surcharger la méthode virtuelle dans la classe de plugin dédiée à l'entité concernée
    /// </remarks>
    public abstract class Plugin : IPlugin
    {
        #region Variables

        protected PluginFeature PluginFeature;
        private readonly string secureConfiguration;
        private readonly string unsecureConfiguration;

        #endregion Variables

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la classe ItlPlugin
        /// </summary>
        protected Plugin()
        { }

        /// <summary>
        /// Initialise une nouvelle instance de la classe ItlPlugin
        /// </summary>
        /// <param name="unsecureConfiguration">Données de configuration publiques</param>
        /// <param name="secureConfiguration">Données de configuration non publiques</param>
        protected Plugin(string unsecureConfiguration, string secureConfiguration)
        {
            this.unsecureConfiguration = unsecureConfiguration;
            this.secureConfiguration = secureConfiguration;

            if (!string.IsNullOrEmpty(unsecureConfiguration))
            {
                PluginFeature =
                    SerializerHelper.ReadObject<PluginFeature>(
                        new MemoryStream(Encoding.Default.GetBytes(unsecureConfiguration)));
            }
        }

        #endregion Constructeur

        #region Exécution

        public void Execute(IServiceProvider serviceProvider)
        {
            var esp = new ExtendedServiceProvider(serviceProvider);

            foreach (var ip in esp.Context.InputParameters)
            {
                if (ip.Value is Entity value)
                {
                    esp.Trace("Input Parameters:");
                    var ipValue = value.ExtractAttributes(null);
                    esp.Trace($"{ip.Key}");
                    esp.Trace(ipValue);
                }
            }

            esp.Trace("");
            esp.Trace("Traitements:");

            try
            {
                switch (esp.Context.Stage)
                {
                    case PluginStage.PreValidation:
                        {
                            switch (esp.Context.MessageName)
                            {
                                case PluginMessage.Create:
                                    {
                                        PreValidationCreate(esp);
                                        PreValidationCreate(new CreationServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.Update:
                                    {
                                        PreValidationUpdate(esp);
                                        PreValidationUpdate(new UpdateServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.Delete:
                                    {
                                        PreValidationDelete(esp);
                                        PreValidationDelete(new DeleteServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.SetState: PreValidationSetState(esp); break;
                                case PluginMessage.SetStateDynamicEntity: PreValidationSetStateDynamicEntity(esp); break;
                                    // Ajouter ici les messages supplémentaires à gérer
                            }
                        }
                        break;

                    case PluginStage.PreOperation:
                        {
                            switch (esp.Context.MessageName)
                            {
                                case PluginMessage.Create:
                                    {
                                        PreOperationCreate(esp);
                                        PreOperationCreate(new CreationServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.Update:
                                    {
                                        PreOperationUpdate(esp);
                                        PreOperationUpdate(new UpdateServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.Delete:
                                    {
                                        PreOperationDelete(esp);
                                        PreOperationDelete(new DeleteServiceProvider(serviceProvider));
                                        break;
                                    }
                                    // Ajouter ici les messages supplémentaires à gérer
                            }
                        }
                        break;

                    case PluginStage.PostOperation:
                        {
                            switch (esp.Context.MessageName)
                            {
                                case PluginMessage.Create:
                                    if (esp.Context.Mode == PluginMode.Synchronous)
                                    {
                                        PostOperationCreate(esp);
                                        PostOperationCreate(new CreationServiceProvider(serviceProvider));
                                    }
                                    else
                                    {
                                        PostOperationCreateAsync(esp);
                                        PostOperationCreateAsync(new CreationServiceProvider(serviceProvider));
                                    }
                                    break;

                                case PluginMessage.Update:
                                    if (esp.Context.Mode == PluginMode.Synchronous)
                                    {
                                        PostOperationUpdate(esp);
                                        PostOperationUpdate(new UpdateServiceProvider(serviceProvider));
                                    }
                                    else
                                    {
                                        PostOperationUpdateAsync(esp);
                                        PostOperationUpdateAsync(new UpdateServiceProvider(serviceProvider));
                                    }
                                    break;

                                case PluginMessage.Delete:
                                    {
                                        PostOperationDelete(esp);
                                        PostOperationDelete(new DeleteServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.SetState: PostOperationSetState(esp); break;
                                case PluginMessage.SetStateDynamicEntity: PostOperationSetStateDynamicEntity(esp); break;
                                case PluginMessage.Associate:
                                    {
                                        PostOperationAssociate(esp);
                                        PostOperationAssociate(new RelationshipServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.Disassociate:
                                    {
                                        PostOperationDisassociate(esp);
                                        PostOperationDisassociate(new RelationshipServiceProvider(serviceProvider));
                                        break;
                                    }
                                case PluginMessage.AddUserToRecordTeam: PostAddUserToRecordTeam(esp); break;
                                case PluginMessage.RemoveUserFromRecordTeam: PostRemoveUserFromRecordTeam(esp); break;

                                    // Ajouter ici les messages supplémentaires à gérer
                            }
                        }
                        break;
                }
            }
            catch (Exception error)
            {
                esp.TracingService.Trace(error.ToString());
                throw new InvalidPluginExecutionException(error.Message);
            }
        }

        #endregion Exécution

        #region Méthodes virtuelles

        public virtual void PostAddUserToRecordTeam(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationAssociate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationAssociate(RelationshipServiceProvider rsp)
        {
        }

        public virtual void PostOperationCreate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationCreate(CreationServiceProvider csp)
        {
        }

        public virtual void PostOperationCreateAsync(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationCreateAsync(CreationServiceProvider csp)
        {
        }

        public virtual void PostOperationDelete(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationDelete(DeleteServiceProvider dsp)
        {
        }

        public virtual void PostOperationDisassociate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationDisassociate(RelationshipServiceProvider rsp)
        {
        }

        public virtual void PostOperationSetState(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationSetStateDynamicEntity(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationUpdate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationUpdate(UpdateServiceProvider usp)
        {
        }

        public virtual void PostOperationUpdateAsync(ExtendedServiceProvider esp)
        {
        }

        public virtual void PostOperationUpdateAsync(UpdateServiceProvider usp)
        {
        }

        public virtual void PostRemoveUserFromRecordTeam(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreOperationCreate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreOperationCreate(CreationServiceProvider csp)
        {
        }

        public virtual void PreOperationDelete(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreOperationDelete(DeleteServiceProvider dsp)
        {
        }

        public virtual void PreOperationUpdate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreOperationUpdate(UpdateServiceProvider usp)
        {
        }

        public virtual void PreValidationCreate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreValidationCreate(CreationServiceProvider csp)
        {
        }

        public virtual void PreValidationDelete(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreValidationDelete(DeleteServiceProvider dsp)
        {
        }

        public virtual void PreValidationSetState(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreValidationSetStateDynamicEntity(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreValidationUpdate(ExtendedServiceProvider esp)
        {
        }

        public virtual void PreValidationUpdate(UpdateServiceProvider usp)
        {
        }

        #endregion Méthodes virtuelles

        #region Méthodes

        /// <summary>
        /// Obtient la valeur de configuration non publique
        /// </summary>
        /// <returns>Valeur de configuration non publique</returns>
        public string GetSecureConfiguration()
        {
            return secureConfiguration;
        }

        /// <summary>
        /// Obtient la valeur de configuration publique
        /// </summary>
        /// <returns>Valeur de configuration publique</returns>
        public string GetUnsecureConfiguration()
        {
            return unsecureConfiguration;
        }

        /// <summary>
        /// Indique si une fonctionnalité doit être activée ou pas. Sans configuration trouvée
        /// dans l'étape de traitement de plugin, la fonctionnalité est considérée comme activée
        /// </summary>
        /// <remarks>
        /// La configuration des fonctionnalités se fait en passant un json comme ci dessous
        /// dans la configuration non sécurisée du plugin:
        /// {"Features":[{"Enabled":true,"Name":"feature1"},{"Enabled":false,"Name":"feature2"}]}
        /// </remarks>
        /// <param name="featureName">Nom de la fonctionnalité</param>
        /// <returns>Indicateur d'état</returns>
        protected bool IsFeatureEnabled(string featureName)
        {
            return PluginFeature?.Features.Any(f => f.Name == featureName && f.Enabled) ?? true;
        }

        #endregion Méthodes
    }
}