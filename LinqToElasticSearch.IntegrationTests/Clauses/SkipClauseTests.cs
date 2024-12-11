﻿using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace LinqToElasticSearch.IntegrationTests.Clauses
{
    public class SkipClauseTests: IntegrationTestsBase<SampleData>
    {
        [Fact]
        public void SkipObjects()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11);

            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = Sut.Skip(5).ToResultList();

            //Then
            results.Count().Should().Be(6);
        }
    }
}