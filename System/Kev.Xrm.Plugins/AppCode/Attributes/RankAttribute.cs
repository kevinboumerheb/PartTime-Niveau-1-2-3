using System;

namespace Kev.Xrm.Plugins.AppCode.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RankAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RankAttribute" /> class.
        /// </summary>
        /// <param name="rank"></param>
        public RankAttribute(int rank)
        {
            Rank = rank;
        }

        /// <summary>
        /// Gets the name of the entity logical.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public int Rank { get; private set; }
    }
}