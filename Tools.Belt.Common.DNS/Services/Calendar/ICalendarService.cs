using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Business.Models;

namespace Tools.Belt.Common.Services.Calendar
{
    public interface ICalendarService :
        IService<int, int, int, CancellationToken>,
        IService<DateTime, DateTime, CancellationToken, bool>,
        IService<bool, DateTime>,
        IService<List<PublicHoliday>>
    {
        int GetFirstWorkingDayOfMonth(ILogger logger, IConfigurationService config, int month, int year,
            CancellationToken token);

        DateTime GetPreviousWorkingDay(ILogger logger, IConfigurationService config, DateTime fromDate,
            CancellationToken token, bool includeCurrentDay = false);

        List<PublicHoliday> GetPublicHolidays(ILogger logger, IConfigurationService config, CancellationToken token);

        bool IsPublicHoliday(ILogger logger, IConfigurationService config, DateTime dateToCheck,
            CancellationToken token);

        DateTime GetIncomeIssueDate(ILogger logger, IConfigurationService config, int month, int year,
            CancellationToken token);
    }
}