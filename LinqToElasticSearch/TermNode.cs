using Elastic.Clients.Elasticsearch.QueryDsl;
namespace LinqToElasticSearch
{
    public class TermNode : Node
    {
        public string Field { get; set; }
        public object Value { get; set; }

        public TermNode(string field, object value)
        {
            Field = field;
            Value = value;
        }

        public override Query Accept(INodeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}