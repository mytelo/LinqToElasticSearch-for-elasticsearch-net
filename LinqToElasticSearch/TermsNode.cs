using System.Collections.Generic;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class TermsNode : Node
    {
        public string Field { get; set; }
        public IEnumerable<object> Values { get; set; }

        public TermsNode(string field, IEnumerable<string> values)
        {
            Field = field;
            Values = values;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}