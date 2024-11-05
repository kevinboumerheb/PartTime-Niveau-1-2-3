using System;

namespace Kev.Xrm.WorkflowActivities.AppCode
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class SourceTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int SourceType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTypeAttribute" /> class.
        /// </summary>
        /// <param name="sourceType"></param>
        public SourceTypeAttribute(int sourceType)
        {
            SourceType = sourceType;
        }
    }
}
