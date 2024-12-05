using Elastic.Clients.Elasticsearch;

namespace LinqToElasticSearch
{
    public class PropertyNameInferrerParser
    {
        private readonly ElasticsearchClient _elasticClient;

        public PropertyNameInferrerParser(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public string Parser(string input)
        {
            return _elasticClient.ElasticsearchClientSettings.DefaultFieldNameInferrer(input);
        }
    }
}