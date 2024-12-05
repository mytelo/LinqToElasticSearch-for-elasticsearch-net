using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class NotNode : Node
    {
        public Node Child { get; set; }

        public NotNode(Node child)
        {
            Child = child;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}