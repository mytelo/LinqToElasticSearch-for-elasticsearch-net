using System.Collections.Generic;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class BoolNode : Node
    {
        public IList<Node> Children { get; set; }

        public BoolNode(IList<Node> children)
        {
            Children = new List<Node>();

            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}