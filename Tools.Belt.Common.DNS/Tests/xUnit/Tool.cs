using System.Diagnostics;
using Xunit;

namespace Tools.Belt.Common.Tests.xUnit
{
    public class ToolAttribute : FactAttribute
    {
        public ToolAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Tools can only be run in debug mode.";
            }
        }
    }
}
