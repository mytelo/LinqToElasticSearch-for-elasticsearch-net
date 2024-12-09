using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class MultiMatchQueryNode : Node
    {
        public MultiMatchQueryConfig MultiMatchQueryConfig { get; set; }

        public MultiMatchQueryNode(MultiMatchQueryConfig multiMatchQueryConfig)
        {
            MultiMatchQueryConfig = multiMatchQueryConfig;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}