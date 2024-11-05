using System;

namespace Kev.Xrm.WorkflowActivities.AppCode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FriendlyNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the friendly name of the workflow custom activity.
        /// </summary>
        /// <value>
        /// The friendly name of the workflow custom activity.
        /// </value>
        public string FriendlyName { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyNameAttribute" /> class.
        /// </summary>
        /// <param name="friendlyName"></param>
        public FriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
    }
}
