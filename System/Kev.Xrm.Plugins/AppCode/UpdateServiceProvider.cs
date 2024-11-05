using Microsoft.Xrm.Sdk;
using System;

namespace Kev.Xrm.Plugins.AppCode
{
    public class UpdateServiceProvider : ExtendedServiceProvider
    {
        public UpdateServiceProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Entity Target => GetInputData();
    }
}