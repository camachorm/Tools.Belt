using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Tools.Belt.Common.Extensions
{
    // ReSharper disable UnusedMember.Global
    public static class XmlStringExtensions
        // ReSharper restore UnusedMember.Global
    {
        // EXPLANATION OF EXPRESSION
        // <        :   \<{1}
        // text     :   (?<xmlTag>\w+)  : xmlTag is a backreference so that the start and end tags match
        // >        :   >{1}
        // xml data :   (?<data>.*)     : data is a backreference used for the regex to return the element data      
        // </       :   <{1}/{1}
        // text     :   \k<xmlTag>
        // >        :   >{1}
        // (\w|\W)* :   Matches attributes if any

        // Sample match and pattern egs
        // Just to show how I incrementally made the patterns so that the final pattern is well-understood
        // <text>data</text>
        // @"^\<{1}(?<xmlTag>\w+)\>{1}.*\<{1}/{1}\k<xmlTag>\>{1}$";

        //<text />
        // @"^\<{1}(?<xmlTag>\w+)\s*/{1}\>{1}$";

        //<text>data</text> or <text />
        // @"^\<{1}(?<xmlTag>\w+)((\>{1}.*\<{1}/{1}\k<xmlTag>)|(\s*/{1}))\>{1}$";

        //<text>data</text> or <text /> or <text attr='2'>xml data</text> or <text attr='2' attr2 >data</text>
        // @"^\<{1}(?<xmlTag>\w+)(((\w|\W)*\>{1}(?<data>.*)\<{1}/{1}\k<xmlTag>)|(\s*/{1}))\>{1}$";

        private const string XmlPattern =
            @"^\<{1}(?<xmlTag>\w+)(((\w|\W)*\>{1}(?<data>.*)\<{1}/{1}\k<xmlTag>)|(\s*/{1}))\>{1}$";

        private static readonly Regex Regex = new Regex(XmlPattern, RegexOptions.Compiled);

        // Checks if the string is in xml format
        public static bool IsXml(this string value)
        {
            return Regex.IsMatch(value);
        }

        /// <summary>
        ///     Assigns the element value to result if the string is xml
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        // ReSharper disable UnusedMember.Global
        public static bool TryParse(this string s, out XElement result)
            // ReSharper restore UnusedMember.Global
        {
            result = null;

            if (!s.IsXml()) return false;

            result = XElement.Parse(s);

            return true;
        }
    }
}