using System;

namespace Kev.Xrm.WorkflowActivities.AppCode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GroupNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the friendly name of the workflow custom activity.
        /// </summary>
        /// <value>
        /// The friendly name of the workflow custom activity.
        /// </value>
        public string GroupName { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupNameAttribute" /> class.
        /// </summary>
        /// <param name="groupName"></param>
        public GroupNameAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
