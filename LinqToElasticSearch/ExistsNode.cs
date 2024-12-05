using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class ExistsNode : Node
    {
        public string Field { get; set; }

        public ExistsNode(string field)
        {
            Field = field;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}