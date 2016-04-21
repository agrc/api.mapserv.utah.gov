#region License

// 
// Copyright (C) 2012 AGRC
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.esriSystem;
using Soe.Common.Extensions;

namespace Wsut.Upgrade.Soe.Extensions
{
    /// <summary>
    ///   Extensions on property sets
    /// </summary>
    public static class PropertySetExensions
    {
        /// <summary>
        ///   Gets the value as string.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="key"> The key. </param>
        /// <param name="errorOnNull"> if set to <c>true</c> [error on null]. </param>
        /// <returns> </returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public static string GetValueAsString(this IPropertySet property, string key, bool errorOnNull = false)
        {
            var value = property.GetProperty(key) as string;

            if (string.IsNullOrEmpty(key))
            {
                var msg = "{0} is null or empty. Please add this value to the properties " +
                          "in the SOE capabilies section of the server manager application.".With(key);

                var logger = new ServerLogger();
                logger.LogMessage(ServerLogger.msgType.warning, "GetPropertyValue", 2472,
                                  msg);
                logger = null;

                if (errorOnNull)
                {
                    throw new NullReferenceException(msg);
                }
            }

            return value ?? "";
        }
    }
}