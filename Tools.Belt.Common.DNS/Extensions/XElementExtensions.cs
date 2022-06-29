using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tools.Belt.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class XElementExtensions
    {
        /// <summary>
        ///     Performs a JObject.Parse(JsonConvert.SerializeXNode(str)) call to convert a XElement to an string instance and then
        ///     parse it into a JObject
        /// </summary>
        /// <param name="str">The XElement to parse</param>
        /// <returns>The parsed JObject</returns>
        public static JObject ToJObject(this XElement str)
        {
            if (str == null) return null;
            // Json.NET XML->JSON->
            return JObject.Parse(JsonConvert.SerializeXNode(str));
        }
    }
}