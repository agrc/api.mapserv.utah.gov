using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebAPI.Common.Extensions
{
    /// <summary>
    ///   Assembly extensions
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        ///   Gets the class types decorated by an attribute in assembly.
        /// </summary>
        /// <param name="assembly"> The assembly to search through. </param>
        /// <param name="attributeType"> The attribute type to find. </param>
        /// <returns> IEnumerable of Types </returns>
        public static IEnumerable<Type> FindTypesWithAttribute(this Assembly assembly, Type attributeType)
        {
            return assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(attributeType, true).Length > 0);
        }
    }
}
