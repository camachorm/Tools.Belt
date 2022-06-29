using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public interface ITableQuerySegmentWrapper<TElement>
    {
        List<TElement> Results { get; }
        TableContinuationToken ContinuationToken { get; }
        IEnumerator<TElement> GetEnumerator();
    }


    public interface ITableQuerySegmentWrapper
    {
        IList<DynamicTableEntity> Results { get; }
        TableContinuationToken ContinuationToken { get; }
        IEnumerator<DynamicTableEntity> GetEnumerator();
    }
}