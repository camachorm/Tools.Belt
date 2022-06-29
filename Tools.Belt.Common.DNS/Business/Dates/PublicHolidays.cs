using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Business.Models;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Business.Dates
{
    public class PublicHolidays : IPublicHolidays
    {
        private const string EmbeddedResourceName = "Tools.Belt.Common.Resources.PublicHolidays.csv";
        private readonly ILogger _logger;
        private List<PublicHoliday> _holidays;

        [ExcludeFromCodeCoverage]
        public PublicHolidays(ILogger logger) : this(
            GetPublicHolidays(
                new List<PublicHoliday>(),
                typeof(PublicHolidays).ReadEmbeddedResource(EmbeddedResourceName),
                logger),
            logger)
        {
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
            Justification = "Cannot check element inside static constructor call")]
        public PublicHolidays(string holidaysCsv, ILogger logger) : this(
            GetPublicHolidays(new List<PublicHoliday>(), holidaysCsv, logger), logger)
        {
        }

        public PublicHolidays(List<PublicHoliday> holidays, ILogger logger)
        {
            _holidays = holidays;
            _logger = logger;
        }

        public List<PublicHoliday> GetPublicHolidays()
        {
            string fileContent = GetType().ReadEmbeddedResource(EmbeddedResourceName);
            _logger.LogTrace($"Loading resource File Holidays: {fileContent.Length}");
            return GetPublicHolidays(fileContent);
        }

        public List<PublicHoliday> GetPublicHolidays(string holidaysCsv)
        {
            if (holidaysCsv == null) throw new ArgumentNullException(nameof(holidaysCsv));

            _logger.LogTrace($"Loading string Holidays: {holidaysCsv.Length}");
            return GetPublicHolidays(_holidays, holidaysCsv, _logger);
        }

        public bool IsPublicHoliday(DateTime dateToCheck)
        {
            if (_holidays == null) _holidays = GetPublicHolidays();

            return _holidays.Any(x => x.Date.Date == dateToCheck.Date);
        }

        public void AddPublicHoliday(DateTime dateToAdd, string description)
        {
            _holidays.Add(
                new PublicHoliday
                {
                    Date = dateToAdd,
                    Description = description
                });
        }

        private static List<PublicHoliday> GetPublicHolidays(
            List<PublicHoliday> holidays,
            string holidaysCsv,
            ILogger logger)
        {
            logger.LogTrace($"Loaded Holidays: {holidays.Count}");
            if (holidays?.Count > 0)
                return holidays;

            logger.LogTrace($"Holidays.Count == 0, Loading fresh from Csv: {holidaysCsv.Length}");

            List<PublicHoliday> data = new List<PublicHoliday>();

            using (StringReader sr = new StringReader(holidaysCsv))
            {
                // Skip Headers Row
                sr.ReadLine();

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] rows = line.Split(',');

                    DateTime.TryParseExact(
                        rows.FirstOrDefault(x => x.Contains("/")),
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime tempDate);

                    data.Add(
                        new PublicHoliday
                        {
                            Description = rows.FirstOrDefault(x => !x.Contains("/")),
                            Date = tempDate
                        });
                }
            }

            logger.LogTrace($"Loaded a total of: {data.Count} public holidays from csv {holidaysCsv.Length}");

            return data;
        }
    }
}