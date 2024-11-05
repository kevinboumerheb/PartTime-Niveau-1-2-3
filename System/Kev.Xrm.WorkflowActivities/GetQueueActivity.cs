using Kev.Xrm.WorkflowActivities.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Linq;

namespace Kev.Xrm.WorkflowActivities
{
    public class GetQueueActivity : BaseWorkflowActivity
    {
        [Input("Adresse Email de la file d'attente")]
        public InArgument<string> Email { get; set; }

        [Input("Nom de la file d'attente")]
        public InArgument<string> Name { get; set; }

        [Output("File d'attente identifiée")]
        [ReferenceTarget("queue")]
        public OutArgument<EntityReference> QueueReference { get; set; }

        public override void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService,
            IWorkflowContext wfContext, ITracingService tracingService)
        {
            tracingService.Trace("Recherche d'une file d'attente");

            var query = new QueryExpression("queue")
            {
                ColumnSet = new ColumnSet("name"),
                Criteria = new FilterExpression(LogicalOperator.And)
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

            var queue = adminService.RetrieveMultiple(query).Entities.ToList().FirstOrDefault();

            if (queue == null)
            {
                throw new InvalidPluginExecutionException($"Aucune file d'attente trouvée pour l'email '{Email.Get(context)}' ou le nom '{Name.Get(context)}'");
            }

            tracingService.Trace($"File d'attente trouvée: {queue.GetAttributeValue<string>("name")} ({queue.Id})");

            QueueReference.Set(context, queue.ToEntityReference());
        }
    }
}