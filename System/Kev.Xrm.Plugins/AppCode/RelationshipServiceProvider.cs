using Microsoft.Xrm.Sdk;
using System;

namespace Kev.Xrm.Plugins.AppCode
{
    public class RelationshipServiceProvider : ExtendedServiceProvider
    {
        public RelationshipServiceProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public EntityReferenceCollection RelatedEntities => GetInputParameter<EntityReferenceCollection>(PluginInputParameters.RelatedEntities);
        public Relationship RelationShip => GetInputParameter<Relationship>(PluginInputParameters.Relationship);
        public EntityReference Target => GetInputParameter<EntityReference>(PluginInputParameters.Target);
    }
}