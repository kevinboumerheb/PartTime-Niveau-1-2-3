using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SecureConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureConfigurationAttribute" /> class.
        /// </summary>
        /// <param name="secureConfigurationAttribute"></param>
        public SecureConfigurationAttribute(string secureConfigurationAttribute)
        {
            SecureConfiguration = secureConfigurationAttribute;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string SecureConfiguration { get; private set; }
    }
}