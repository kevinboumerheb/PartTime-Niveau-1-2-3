using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SupportedDeploymentAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDeploymentAttribute" /> class.
        /// </summary>
        /// <param name="supportedDeployment"></param>
        public SupportedDeploymentAttribute(int supportedDeployment)
        {
            SupportedDeployment = supportedDeployment;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int SupportedDeployment { get; private set; }
    }
}