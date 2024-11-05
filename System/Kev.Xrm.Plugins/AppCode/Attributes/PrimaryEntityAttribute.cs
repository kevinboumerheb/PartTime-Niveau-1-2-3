using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PrimaryEntityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryEntityAttribute" /> class.
        /// </summary>
        /// <param name="primaryEntity"></param>
        public PrimaryEntityAttribute(string primaryEntity)
        {
            PrimaryEntity = primaryEntity;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string PrimaryEntity { get; private set; }
    }
}