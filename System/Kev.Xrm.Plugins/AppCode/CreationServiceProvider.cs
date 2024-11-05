using Microsoft.Xrm.Sdk;
using System;

namespace Kev.Xrm.Plugins.AppCode
{
    public class CreationServiceProvider : ExtendedServiceProvider
    {
        public CreationServiceProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Guid Id => GetOutputParameter<Guid>(PluginOutputParameters.Id);
        public Entity Target => GetInputData();
    }
}