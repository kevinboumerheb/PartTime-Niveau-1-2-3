using Kev.Xrm.WorkflowActivities.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Linq;

namespace Kev.Xrm.WorkflowActivities
{
    public class GetTeamActivity : BaseWorkflowActivity
    {
        [Input("Adresse Email de l'équipe")]
        public InArgument<string> Email { get; set; }

        [Input("Nom de l'équipe")]
        public InArgument<string> Name { get; set; }

        [Output("Equipe identifiée")]
        [ReferenceTarget("team")]
        public OutArgument<EntityReference> TeamReference { get; set; }

        public override void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService,
            IWorkflowContext wfContext, ITracingService tracingService)
        {
            tracingService.Trace("Recherche d'une équipe");

            var query = new QueryExpression("team")
            {
                ColumnSet = new ColumnSet("name"),
                Criteria = new FilterExpression(LogicalOperator.Or)
            };

            if (!string.IsNullOrEmpty(Email.Get(context)))
            {
                tracingService.Trace($"Email: {Email.Get(context)}");
                query.Criteria.AddCondition("emailaddress", ConditionOperator.Equal, Email.Get(context));
            }

            if (!string.IsNullOrEmpty(Name.Get(context)))
            {
                tracingService.Trace($"Nom: {Name.Get(context)}");
                query.Criteria.AddCondition("name", ConditionOperator.Equal, Name.Get(context));
            }

            var team = adminService.RetrieveMultiple(query).Entities.ToList().FirstOrDefault();

            if (team == null)
            {
                throw new InvalidPluginExecutionException($"Aucune équipe trouvée pour l'email '{Email.Get(context)}' ou le nom '{Name.Get(context)}'");
            }

            tracingService.Trace($"Equipe trouvée: {team.GetAttributeValue<string>("name")} ({team.Id})");

            TeamReference.Set(context, team.ToEntityReference());
        }
    }
}