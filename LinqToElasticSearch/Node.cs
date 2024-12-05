using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public abstract class Node
    {
        public abstract Query Accept(INodeVisitor visitor);
    }
}