using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionAttribute" /> class.
        /// </summary>
        /// <param name="description"></param>
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string Description { get; private set; }
    }
}