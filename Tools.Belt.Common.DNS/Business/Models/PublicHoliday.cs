using System;
using System.Diagnostics.CodeAnalysis;

namespace Tools.Belt.Common.Business.Models
{
    [ExcludeFromCodeCoverage]
    public class PublicHoliday
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}