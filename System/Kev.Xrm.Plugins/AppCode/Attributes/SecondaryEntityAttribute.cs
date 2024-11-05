using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SecondaryEntityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecondaryEntityAttribute" /> class.
        /// </summary>
        /// <param name="secondaryEntity"></param>
        public SecondaryEntityAttribute(string secondaryEntity)
        {
            SecondaryEntity = secondaryEntity;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string SecondaryEntity { get; private set; }
    }
}