using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class QueryMatchNode : Node
    {
        public QueryConfig QueryConfig { get; set; }


        public QueryMatchNode(QueryConfig queryConfig)
        {
            QueryConfig = queryConfig;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}