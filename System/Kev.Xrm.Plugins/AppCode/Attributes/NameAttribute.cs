using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameAttribute" /> class.
        /// </summary>
        /// <param name="name"></param>
        public NameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string Name { get; private set; }
    }
}