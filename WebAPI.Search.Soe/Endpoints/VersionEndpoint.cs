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

using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace WebAPI.Search.Soe.Endpoints
{
    /// <summary>
    ///   Displays the information about the soe.
    /// </summary>
    public static class VersionEndpoint
    {
        /// <summary>
        ///   Handlers the rest request for when the soe page is viewed.
        /// </summary>
        /// <param name="boundVariables"> The bound variables. </param>
        /// <param name="outputFormat"> The output format. </param>
        /// <param name="requestProperties"> The request properties. </param>
        /// <param name="responseProperties"> The response properties. </param>
        /// <returns> </returns>
        public static byte[] Handler(NameValueCollection boundVariables, string outputFormat, string requestProperties,
                                     out string responseProperties)
        {
            responseProperties = null;

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                                                                      {
                                                                          Description = "Web API Searching",
                                                                          CreatedBy = "AGRC - Steve Gourley @steveagrc",
                                                                          Version = Assembly.GetExecutingAssembly().GetName().
                                                                          Version.ToString()
                                                                      }));
        }
    }
}