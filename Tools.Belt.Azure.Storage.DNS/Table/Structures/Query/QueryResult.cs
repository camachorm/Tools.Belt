using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Tools.Belt.Azure.Storage.DNS.Table.Structures.Query
{
    public class QueryResult<T> where T : ITableEntity, new()
    {
        public QueryResult(T entity, TableContinuationToken token, string filter, TimeSpan queryTime)
        {
            Entity = entity;
            ContinuationToken = token;
            Filter = filter;
            QueryTime = queryTime;
        }

        public TimeSpan QueryTime { get; set; } = new TimeSpan();

        public T Entity { get; set; }

        public TableContinuationToken ContinuationToken { get; set; }

        public string Filter;
    }
}