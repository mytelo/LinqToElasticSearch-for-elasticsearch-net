using System;
using System.Collections.Generic;
using AutoFixture;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using FluentAssertions;
using LinqToElasticSearch.Config;
using Newtonsoft.Json;

namespace LinqToElasticSearch.IntegrationTests
{
    public abstract class IntegrationTestsBase<T> : IDisposable where T : class
    {
        protected readonly ElasticsearchClient ElasticClient;
        protected readonly ElasticQueryable<T> Sut;
        private readonly string IndexName = $"linqtoelasticsearch-{typeof(T).Name.ToLower()}";
        protected Fixture Fixture { get; }

        protected IntegrationTestsBase()
        {
            Fixture = new Fixture();

            var server = GetSettingsValue("ElasticSearch.ReadModelNodeList", "http://192.168.168.6:9200");
            var username = GetSettingsValue("ElasticSearch.UserName", "");
            var password = GetSettingsValue("ElasticSearch.Password", "");

            ElasticClient = ElasticClientFactory.CreateElasticClient(new EsConfig
            {
                Urls = server,
                UserName = username,
                Password = password
            });

            if (ElasticClient.Indices.Exists(IndexName).Exists)
            {
                ElasticClient.Indices.Delete(IndexName);
            }

            var map = new TypeMappingDescriptor<SampleData>();
            map.Properties(p => p
                .Text(t => t.Name, c=>c.Fields(p2=>p2.Keyword("keyword")))
                .Text(t => t.LastName, c => c.Fields(p2 => p2.Keyword("keyword")))
                .Keyword(t => t.Id)
                .Keyword(t => t.Emails)
                .Keyword(t => t.FolderId)
                .Keyword(t => t.SampleTypePropertyString)
                .Keyword(t => t.TypeId)
                .IntegerNumber(t => t.EnumNullable)
                .IntegerNumber(t => t.SampleTypeProperty)
                .IntegerNumber(t => t.Age)
                .Date(t => t.Date)
                .Date(t => t.Date1)
                .Date(t => t.DateOffset)
                .Date(t => t.DateOffset1)
                .Boolean(t => t.Can)
                .LongNumber(t => t.TimeSpan)
                .LongNumber(t => t.TimeSpanNullable)
            );


            ElasticClient.Indices.Create<SampleData>(IndexName, c => c.Mappings(map));

            Sut = new ElasticQueryable<T>(ElasticClient, IndexName);

        }

        protected void Bulk(IEnumerable<T> datas)
        {
            var result = ElasticClient.BulkAsync(descriptor => descriptor.Index(IndexName).IndexMany(datas)).Result;
        }

        protected void Index(T data)
        {
            ElasticClient.IndexAsync(data, descriptor => descriptor.Index(IndexName)).Wait();
        }

        public void Dispose()
        {
            ElasticClient.Indices.DeleteAsync(IndexName).Wait();
        }

        private string GetSettingsValue(string key, string defaultValue)
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue;
        }

        public void CompareAsJson(object obj1, object obj2)
        {
            JsonConvert.SerializeObject(obj1).Should().Be(JsonConvert.SerializeObject(obj2));
        }
    }
}