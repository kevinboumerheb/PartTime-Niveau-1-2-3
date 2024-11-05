using System.Activities;
using Kev.Xrm.WorkflowActivities.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Kev.Xrm.WorkflowActivities
{
    [GroupName("Un groupe")]
    [FriendlyName("Une activité d'exemple")]
    public class SampleWorkflowActivity : BaseWorkflowActivity
    {
        [Input("Valeur entrante")]
        [RequiredArgument]
        public InArgument<string> InputValue { get; set; }

        [Output("Valeur sortante")]
        public InArgument<string> OutputValue { get; set; }

        public override void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService,
            IWorkflowContext wfContext, ITracingService tracingService)
        {
            InputValue.Set(context, InputValue.Get(context).ToUpper());
        }
    }
}
