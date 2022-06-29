using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table.Structures.Query
{
    public class AllQueryResult<T> where T : ITableEntity, new()
    {
        public AllQueryResult()
        {
        }

        public AllQueryResult(
            IList<T> records,
            TableContinuationToken continuationToken,
            string filterString,
            bool started,
            IEnumerable<TimeSpan> queryTimes)
        {
            Records = records;
            ContinuationToken = continuationToken;
            FilterString = filterString;
            QueryTimes = queryTimes.ToList();
            Started = started;
        }

        public AllQueryResult(
            IList<T> records,
            TableContinuationToken continuationToken,
            string filterString,
            bool started,
            params TimeSpan[] queryTimes)
            : this(records, continuationToken, filterString, started, (IEnumerable<TimeSpan>)queryTimes)
        {
        }

        public IList<TimeSpan> QueryTimes { get; set; }

        public IList<T> Records { get; set; }

        public TableContinuationToken ContinuationToken { get; set; }

        public string FilterString;

        /// <summary>
        /// Indicates whether we queried table storage already at least once.
        /// </summary>
        public bool Started { get; set; }
    }
}