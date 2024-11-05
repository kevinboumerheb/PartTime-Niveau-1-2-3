using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FilteringAttributesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringAttributesAttribute" /> class.
        /// </summary>
        /// <param name="filteringAttributes"></param>
        public FilteringAttributesAttribute(string filteringAttributes)
        {
            FilteringAttributes = filteringAttributes;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string FilteringAttributes { get; private set; }
    }
}