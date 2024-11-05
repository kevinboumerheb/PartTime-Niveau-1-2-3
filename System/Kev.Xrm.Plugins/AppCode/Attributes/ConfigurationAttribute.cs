using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationAttribute" /> class.
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigurationAttribute(string configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string Configuration { get; private set; }
    }
}