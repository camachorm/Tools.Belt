using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public class TableQuerySegmentWrapper<TElement> : ITableQuerySegmentWrapper<TElement>
    {
        private readonly TableQuerySegment<TElement> _tagQuerySegment;


        public TableQuerySegmentWrapper()
        {
            
        }

        public TableQuerySegmentWrapper(IEnumerable<TElement> records)
        {
            Results = records.ToList();
        }

        public TableQuerySegmentWrapper(TableQuerySegment<TElement> tagQuerySegment)
        {
            _tagQuerySegment = tagQuerySegment;
            Results = tagQuerySegment.Results;
        }
        public List<TElement> Results { get; set; }

        public TableContinuationToken ContinuationToken => _tagQuerySegment?.ContinuationToken;
        public IEnumerator<TElement> GetEnumerator()
        {
            if (_tagQuerySegment != null) return _tagQuerySegment.GetEnumerator();
            return new List<TElement>().GetEnumerator();
            
        }
    }

    public class TableQuerySegmentWrapper : ITableQuerySegmentWrapper
    {
        private readonly IList<DynamicTableEntity> _records = new List<DynamicTableEntity>();
        private readonly TableQuerySegment _tagQuerySegment;
        
        public TableQuerySegmentWrapper()
        {

        }
        
        public TableQuerySegmentWrapper(IList<DynamicTableEntity> records)
        {
            _records = records;
        }
        public TableQuerySegmentWrapper(TableQuerySegment tagQuerySegment)
        {
            _tagQuerySegment = tagQuerySegment;
        }
        public IList<DynamicTableEntity> Results => _tagQuerySegment != null ? _tagQuerySegment.Results : _records;
        public TableContinuationToken ContinuationToken => _tagQuerySegment?.ContinuationToken;
        public IEnumerator<DynamicTableEntity> GetEnumerator()
        {
            if (_tagQuerySegment != null) return _tagQuerySegment.GetEnumerator();
            return Results.GetEnumerator();

        }
    }

}