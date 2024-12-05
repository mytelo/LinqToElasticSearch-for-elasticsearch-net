using System.Linq;
using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace LinqToElasticSearch
{
    public class ElasticQueryable<T> : QueryableBase<T>
    {
        public ElasticQueryable(ElasticsearchClient elasticClient, string dataId)
            : base(new DefaultQueryProvider(typeof(ElasticQueryable<>), QueryParser.CreateDefault(), new ElasticQueryExecutor<T>(elasticClient, dataId)))
        {
        }

        public ElasticQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}