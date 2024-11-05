using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class IsolationModeAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int IsolationMode { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IsolationModeAttribute" /> class.
        /// </summary>
        /// <param name="isolationMode"></param>
        public IsolationModeAttribute(int isolationMode)
        {
            IsolationMode = isolationMode;
        }
    }
}
