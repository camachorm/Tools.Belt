using System;
using System.Collections.Generic;
using Tools.Belt.Common.Business.Models;

namespace Tools.Belt.Common.Business.Dates
{
    public interface IPublicHolidays
    {
        List<PublicHoliday> GetPublicHolidays();
        List<PublicHoliday> GetPublicHolidays(string holidaysCsv);
        bool IsPublicHoliday(DateTime dateToCheck);
        void AddPublicHoliday(DateTime dateToAdd, string description);
    }
}