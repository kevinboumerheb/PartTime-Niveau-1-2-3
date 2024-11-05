using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Kev.Xrm.WorkflowActivities.AppCode
{
    /// <summary>
    /// Cette classe sert de base pour développer les activités de workflow
    /// </summary>
    /// <remarks>
    /// Cette classe ne doit jamais être modifiée
    /// </remarks>
    public abstract class BaseWorkflowActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var tracingService = context.GetExtension<ITracingService>();

            try
            {
                var executionContext = context.GetExtension<IWorkflowContext>();
                var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
                var userService = serviceFactory.CreateOrganizationService(executionContext.UserId);
                var adminService = serviceFactory.CreateOrganizationService(null);
                
                Execute(context, userService, adminService, executionContext, tracingService);
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        public abstract void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService, IWorkflowContext wfContext, ITracingService tracingService);
    }
}
