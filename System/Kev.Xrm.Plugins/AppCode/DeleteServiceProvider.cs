using Microsoft.Xrm.Sdk;
using System;

namespace Kev.Xrm.Plugins.AppCode
{
    public class DeleteServiceProvider : ExtendedServiceProvider
    {
        public DeleteServiceProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public EntityReference Target => GetInputParameter<EntityReference>(PluginInputParameters.Target);
    }
}