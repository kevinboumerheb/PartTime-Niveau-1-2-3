using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InvocationSourceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationSourceAttribute" /> class.
        /// </summary>
        /// <param name="invocationSource"></param>
        public InvocationSourceAttribute(int invocationSource)
        {
            InvocationSource = invocationSource;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int InvocationSource { get; private set; }
    }
}