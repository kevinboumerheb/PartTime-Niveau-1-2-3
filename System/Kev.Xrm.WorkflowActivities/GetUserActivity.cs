using Kev.Xrm.WorkflowActivities.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Linq;

namespace Kev.Xrm.WorkflowActivities
{
    public class GetUserActivity : BaseWorkflowActivity
    {
        [Input("Adresse Email de l'utilisateur")]
        public InArgument<string> Email { get; set; }

        [Input("Nom complet de l'utilisateur")]
        public InArgument<string> Fullname { get; set; }

        [Output("Utilisateur identifié")]
        [ReferenceTarget("systemuser")]
        public OutArgument<EntityReference> UserReference { get; set; }

        public override void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService,
            IWorkflowContext wfContext, ITracingService tracingService)
        {
            tracingService.Trace("Recherche d'un utilisateur");

            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("fullname"),
                Criteria = new FilterExpression(LogicalOperator.Or)
            };

            if (!string.IsNullOrEmpty(Email.Get(context)))
            {
                tracingService.Trace($"Email: {Email.Get(context)}");
                query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, Email.Get(context));
            }

            if (!string.IsNullOrEmpty(Fullname.Get(context)))
            {
                tracingService.Trace($"Nom complet: {Fullname.Get(context)}");
                query.Criteria.AddCondition("fullname", ConditionOperator.Equal, Fullname.Get(context));
            }

            var user = adminService.RetrieveMultiple(query).Entities.ToList().FirstOrDefault();

            if (user == null)
            {
                throw new InvalidPluginExecutionException($"Aucun utilisateur trouvé pour l'email '{Email.Get(context)}' ou le nom '{Fullname.Get(context)}'");
            }

            tracingService.Trace($"Utilisateur trouvé: {user.GetAttributeValue<string>("fullname")} ({user.Id})");

            UserReference.Set(context, user.ToEntityReference());
        }
    }
}