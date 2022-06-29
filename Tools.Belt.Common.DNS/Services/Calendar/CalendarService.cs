// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Business.Dates;
using Tools.Belt.Common.Business.Models;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Logging;

namespace Tools.Belt.Common.Services.Calendar
{
    public class CalendarService : ICalendarService
    {
        private readonly IPublicHolidays _publicHolidays;

        public CalendarService(ILogger logger = null) : this(
            new PublicHolidays(logger ?? StaticLoggerFactory.CreateLogger("CalendarServiceDebugLogger")))
        {
        }

        public CalendarService(IPublicHolidays publicHolidays)
        {
            _publicHolidays = publicHolidays;
        }

        public int GetFirstWorkingDayOfMonth(ILogger logger, IConfigurationService config, int month, int year,
            CancellationToken token)
        {
            Task<int> t = ExecuteAsync(logger, config, month, year, token);
            t.Wait(token);
            return t.Result;
        }

        public DateTime GetPreviousWorkingDay(ILogger logger, IConfigurationService config, DateTime fromDate,
            CancellationToken token, bool includeCurrentDay = false)
        {
            Task<DateTime> t = ExecuteAsync(logger, config, fromDate, token, includeCurrentDay);
            t.Wait(token);
            return t.Result;
        }

        public List<PublicHoliday> GetPublicHolidays(ILogger logger, IConfigurationService config,
            CancellationToken token)
        {
            Task<List<PublicHoliday>> t = ExecuteAsync(logger, config);
            t.Wait(token);
            return t.Result;
        }

        public bool IsPublicHoliday(ILogger logger, IConfigurationService config, DateTime dateToCheck,
            CancellationToken token)
        {
            Task<bool> t = ExecuteAsync(logger, config, dateToCheck);
            t.Wait(token);
            return t.Result;
        }

        public DateTime GetIncomeIssueDate(ILogger logger, IConfigurationService config, int month, int year,
            CancellationToken token)
        {
            Task<DateTime> t = ExecuteAsync(logger, config, month, year);
            t.Wait(token);
            return t.Result;
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public Task<int> ExecuteAsync(ILogger logger, IConfigurationService config, int month, int year,
            CancellationToken input3)
        {
            logger.LogTrace("Getting First Working Day Of Month");
            DateTime calcDate = new DateTime(year, month, 1);

            while (_publicHolidays.IsPublicHoliday(calcDate) || calcDate.IsWeekend()) calcDate = calcDate.AddDays(1);

            return Task.FromResult(calcDate.Day);
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public Task<DateTime> ExecuteAsync(ILogger logger, IConfigurationService config, DateTime fromDate,
            CancellationToken input3, bool includeCurrentDay = false)
        {
            logger.LogTrace("Getting Previous Working Day");

            if (!includeCurrentDay)
                fromDate = fromDate.AddDays(-1);

            while (_publicHolidays.IsPublicHoliday(fromDate) || fromDate.IsWeekend()) fromDate = fromDate.AddDays(-1);

            return Task.FromResult(fromDate);
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public Task<bool> ExecuteAsync(ILogger logger, IConfigurationService config, DateTime date)
        {
            logger.LogTrace("Validate Public Holiday");
            return Task.FromResult(_publicHolidays.IsPublicHoliday(date));
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public Task<List<PublicHoliday>> ExecuteAsync(ILogger logger, IConfigurationService config)
        {
            logger.LogTrace("Get Public Holidays");
            return Task.FromResult(_publicHolidays.GetPublicHolidays());
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Maintaining design consistency.")]
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Maintaining design consistency.")]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Discard element")]
        public Task<DateTime> ExecuteAsync(ILogger logger, IConfigurationService _, int month, int year)
        {
            logger.LogTrace("Getting Income Issue Date");
            DateTime incomeAvailableDate = new DateTime(year, month, 1).AddMonths(1);
            int counter = 1;
            while (_publicHolidays.IsPublicHoliday(incomeAvailableDate) || incomeAvailableDate.IsWeekend())
                incomeAvailableDate = incomeAvailableDate.AddDays(-1);
            DateTime incomeIssueDate = incomeAvailableDate;

            while (counter <= 5)
            {
                incomeIssueDate = incomeIssueDate.AddDays(-1);

                if (!(_publicHolidays.IsPublicHoliday(incomeIssueDate) || incomeIssueDate.IsWeekend())) counter += 1;
            }

            return Task.FromResult(incomeIssueDate);
        }
    }
}