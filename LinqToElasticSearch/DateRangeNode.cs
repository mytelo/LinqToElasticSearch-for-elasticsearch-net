using System;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class DateRangeNode : Node
    {
        public string Field { get; set; }
        public DateTime? GreaterThan { get; set; }
        public DateTime? GreaterThanOrEqualTo { get; set; }
        public DateTime? LessThan { get; set; }
        public DateTime? LessThanOrEqualTo { get; set; }

        public DateRangeNode(string field)
        {
            Field = field;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}