using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StageAttribute" /> class.
        /// </summary>
        /// <param name="stage"></param>
        public StageAttribute(int stage)
        {
            Stage = stage;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int Stage { get; private set; }
    }
}