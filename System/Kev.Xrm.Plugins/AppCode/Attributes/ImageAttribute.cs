using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ImageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAttribute" /> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageType"></param>
        /// <param name="attributes"></param>
        public ImageAttribute(string name, int imageType, string attributes)
        {
            Name = name;
            ImageType = imageType;
            Attributes = attributes;
        }

        public string Attributes { get; private set; }

        public int ImageType { get; private set; }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string Name { get; private set; }
    }
}