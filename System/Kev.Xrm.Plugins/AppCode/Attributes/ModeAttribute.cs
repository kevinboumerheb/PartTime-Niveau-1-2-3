using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModeAttribute" /> class.
        /// </summary>
        /// <param name="mode"></param>
        public ModeAttribute(int mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int Mode { get; private set; }
    }
}