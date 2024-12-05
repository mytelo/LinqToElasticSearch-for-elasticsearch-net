using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class NotExistsNode : Node
    {
        public string Field { get; set; }

        public NotExistsNode(string field)
        {
            Field = field;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}