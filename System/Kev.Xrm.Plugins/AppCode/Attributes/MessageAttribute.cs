using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MessageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute" /> class.
        /// </summary>
        /// <param name="message"></param>
        public MessageAttribute(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string Message { get; private set; }
    }
}