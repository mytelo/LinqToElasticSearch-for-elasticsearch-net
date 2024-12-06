using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class QueryMatchNode : Node
    {
        public string Field { get; set; }
        public object Value { get; set; }
        public QueryConfig QueryConfig { get; set; }


        public QueryMatchNode(string field, object value, QueryConfig queryConfig)
        {
            Field = field;
            Value = value;
            QueryConfig = queryConfig;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}