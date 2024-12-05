using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;
using LinqToElasticSearch.Config;
using TimeSpanConverter = LinqToElasticSearch.JsonConverter.TimeSpanConverter;

namespace LinqToElasticSearch
{
    public static class ElasticClientFactory
    {
        static void ConfigureOptions(JsonSerializerOptions o)
        {
            //var jsonStringEnumConverter = o.Converters.FirstOrDefault(c => c.GetType().Name == "JsonStringEnumConverter");
            //if (jsonStringEnumConverter != null)
            //{
            //    //remove default jsonConverter
            //    o.Converters.RemoveAt(o.Converters.IndexOf(jsonStringEnumConverter));
            //    o.Converters.Add(new EnumConverter());
            //}
            o.Converters.Add(new TimeSpanConverter());
        }

        public static ElasticsearchClient CreateElasticClient(EsConfig esConfig)
        {
            var uris = esConfig.Uris;
            if (uris == null || uris.Count < 1)
            {
                throw new Exception("urls can not be null");
            }

            ElasticsearchClientSettings connectionSetting;
            if (uris.Count == 1)
            {
                var uri = uris.First();
                var nodePool = new SingleNodePool(uri);
                connectionSetting = new ElasticsearchClientSettings(nodePool, sourceSerializer: (defaultSerializer, settings) =>
                    new DefaultSourceSerializer(settings, ConfigureOptions));
            }
            else
            {
                var connectionPool = new SniffingNodePool(uris);
                connectionSetting = new ElasticsearchClientSettings(connectionPool, sourceSerializer: (defaultSerializer, settings) =>
                    new DefaultSourceSerializer(settings, ConfigureOptions)).DefaultIndex("");
            }

            if (!string.IsNullOrWhiteSpace(esConfig.UserName) && !string.IsNullOrWhiteSpace(esConfig.Password))
                connectionSetting.Authentication(new BasicAuthentication(esConfig.UserName, esConfig.Password));
#if DEBUG
            connectionSetting.EnableDebugMode(details =>
            {
                Console.WriteLine($"ES Request: {Encoding.UTF8.GetString(details.RequestBodyInBytes ?? new byte[0])}");
                Console.WriteLine($"ES Response: {Encoding.UTF8.GetString(details.ResponseBodyInBytes ?? new byte[0])}");
            });
#endif

            return new ElasticsearchClient(connectionSetting);
            
        }
    }
}