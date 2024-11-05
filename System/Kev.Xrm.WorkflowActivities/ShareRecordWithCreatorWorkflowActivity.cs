using System.Activities;
using Kev.Xrm.WorkflowActivities.AppCode;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk.Messages;
using System;

namespace Kev.Xrm.WorkflowActivities
{
    public class ShareRecordWithCreatorWorkflowActivity : BaseWorkflowActivity
    {
        [Input("Record")]
        [ReferenceTarget("account")]  // This workflow works for the Account entity (can be generalized if needed)
        public InArgument<EntityReference> Record { get; set; }

        [Output("Result")]
        public OutArgument<bool> Result { get; set; }

        public override void Execute(CodeActivityContext context, IOrganizationService userService, IOrganizationService adminService,
            IWorkflowContext wfContext, ITracingService tracingService)
        {
            try
            {
                // Get the record reference from input
                var recordRef = Record.Get(context);
                if (recordRef == null)
                {
                    tracingService.Trace("No record found.");
                    Result.Set(context, false);
                    return;
                }

                // Retrieve the full record to access the owner and creator fields
                var record = adminService.Retrieve(recordRef.LogicalName, recordRef.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("ownerid", "createdby"));

                // Get the creator's ID and the current owner
                var creatorId = record.GetAttributeValue<EntityReference>("createdby").Id;
                var currentOwnerId = record.GetAttributeValue<EntityReference>("ownerid").Id;

                tracingService.Trace($"Creator ID: {creatorId}, Current Owner ID: {currentOwnerId}");

                // Check if the current owner is different from the creator
                if (creatorId != currentOwnerId)
                {
                    tracingService.Trace("Sharing record with the creator...");

                    // Create a request to share the record with the creator
                    var accessRequest = new GrantAccessRequest
                    {
                        Target = recordRef,  // Use the record reference as the target
                        PrincipalAccess = new PrincipalAccess
                        {
                            Principal = new EntityReference("systemuser", creatorId),
                            AccessMask = AccessRights.WriteAccess  // Grant write access
                        }
                    };

                    // Execute the request to share the record
                    adminService.Execute(accessRequest);

                    // Indicate that sharing was successful
                    tracingService.Trace("Record successfully shared with the creator.");
                    Result.Set(context, true);
                }
                else
                {
                    // No sharing needed if the current owner is the creator
                    tracingService.Trace("Owner is the same as the creator, no sharing needed.");
                    Result.Set(context, false);
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception: {ex.Message}");
                throw new InvalidPluginExecutionException($"An error occurred in ShareRecordWithCreatorWorkflowActivity: {ex.Message}", ex);
            }
        }
    }
}
