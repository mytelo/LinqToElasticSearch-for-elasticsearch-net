﻿using System.Collections.Generic;
using AutoFixture;
using LinqToElk.IntegrationTests.Utils;
using Nest;

namespace LinqToElk.IntegrationTests
{
    public abstract class IntegrationTestsBase<T> where T : class
    {
        protected readonly ElasticClient ElasticClient;
        protected ElasticQueryable<T> Sut;
        protected const string DataId = "123456789";

        public Fixture Fixture { get; set; }

        protected IntegrationTestsBase()
        {
            Fixture = new Fixture();
            
            ElasticClient = ElasticClientFactory.CreateElasticClient("http://localhost:9200", "", ""); 
            
            if (ElasticClient.Indices.Exists(DataId).Exists)
            {
                ElasticClient.Indices.Delete(DataId);
            }
            
            Sut = new ElasticQueryable<T>(ElasticClient, DataId);
        }

        protected void Bulk(IEnumerable<T> datas)
        {
            ElasticClient.Bulk(descriptor => descriptor.Index(DataId).IndexMany(datas));
        }
        
        protected void Index(T data)
        {
            ElasticClient.Index(data, descriptor => descriptor.Index(DataId));
        }
    }
}