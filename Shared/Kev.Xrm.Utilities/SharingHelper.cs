using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Kev.Xrm.Utilities
{
    public class SharingHelper
    {
        public static void Share(IOrganizationService service, EntityReference principal, AccessRights mask,
            EntityReference target)
        {
            service.Execute(new GrantAccessRequest
            {
                Target = target,
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = mask,
                    Principal = principal
                }
            });
        }
    }
}