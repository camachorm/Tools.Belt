using System.Collections.Generic;
using System.Diagnostics;

namespace Tools.Belt.Azure.Storage.DNS.Table.Structures.Query
{
    public class TableQueryOptions
    {
        public TableQueryOptions(IList<string> selectColumns = null, 
            int maxIterations = 1,
            int? numberOfRecordsToTake = null,
            Stopwatch queryTimer = null,
            int? maxQueryRunningSeconds = null)
        {
            SelectColumns = selectColumns;
            MaxIterations = maxIterations;
            NumberOfRecordsToTake = numberOfRecordsToTake;
            QueryTimer = queryTimer;
            MaxQueryRunningSeconds = maxQueryRunningSeconds;
        }

        /// <summary>
        /// Table columns to select (if not set all columns are selected).
        /// </summary>
        public IList<string> SelectColumns { get; set; }

        /// <summary>
        /// Maximum consequtive number of requests using continuation tokens for partial query within one
        /// API request.
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Maximum number of records to retrieve from table storage.
        /// </summary>
        public int? NumberOfRecordsToTake { get; set; }

        /// <summary>
        /// Measures time elapsed from the start of the API request.
        /// </summary>
        public Stopwatch QueryTimer { get; set; }

        /// <summary>
        /// Maximum running time for the API query request (continuation token is sent if the request is not completed
        /// in time).
        /// </summary>
        public int? MaxQueryRunningSeconds { get; set; }
    }
}
