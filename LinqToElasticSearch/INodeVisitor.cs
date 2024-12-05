using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public interface INodeVisitor
    {
        Query Visit(BoolNode node);
        Query Visit(OrNode node);
        Query Visit(AndNode node);
        Query Visit(NotNode node);
        Query Visit(TermNode node);
        Query Visit(TermsNode node);
        Query Visit(TermsSetNode node);
        Query Visit(ExistsNode node);
        Query Visit(NotExistsNode node);
        Query Visit(DateRangeNode node);
        Query Visit(MatchPhraseNode node);
        Query Visit(NumericRangeNode node);
        Query Visit(QueryStringNode node);
        Query Visit(MultiMatchNode node);
    }
}