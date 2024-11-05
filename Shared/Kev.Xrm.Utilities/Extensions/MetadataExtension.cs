using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;

namespace Kev.Xrm.Utilities.Extensions
{
    public static class MetadataExtension
    {
        public static string GetOptionsetLabel(this IOrganizationService service, string entityLogicalName, string field, int optionSetValue)
        {
            var attReq = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = field,
                RetrieveAsIfPublished = true
            };
            var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
            return attMetadata.OptionSet.Options.FirstOrDefault(x => x.Value == optionSetValue)?.Label.UserLocalizedLabel.Label;
        }

        public static OptionMetadataCollection GetOptionsetLabels(this IOrganizationService service, string entityLogicalName, string field)
        {
            var attReq = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = field,
                RetrieveAsIfPublished = true
            };
            var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
            return attMetadata.OptionSet.Options;
        }
    }
}