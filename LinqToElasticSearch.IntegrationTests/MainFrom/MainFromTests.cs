﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace LinqToElasticSearch.IntegrationTests.MainFrom
{
    public class MainFromTests : IntegrationTestsBase<SampleData>
    {
        [Fact]
        public void MustSearchWithMainFromFormat()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Id = null;
            }
            
            
            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = (from i in Sut 
                where i.Id == null
                orderby i.Name, i.Age
                select i)
                .Skip(5).Take(3);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(3);
        }
        
        [Fact]
        public void MustSearchWithMainFromFormat2()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Name = null;
            }
            
            
            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = (from i in Sut 
                    where i.Name == null
                    orderby i.Date, i.Age
                    select i.Id)
                .Skip(5).Take(3);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(3);
        }
        
        [Fact]
        public void MustSearchWithMainFromFormat3()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Name = null;
            }
            
            
            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = (from i in Sut 
                    where i.Name == null
                    orderby i.Date, i.Age
                    select i)
                .Skip(5).Take(3).Select(x => x.Id);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(3);
        }
        
        
        [Fact]
        public void MustSearchWithMainFromFormat4()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Name = null;
            }
            
            
            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = (from i in Sut 
                    where i.Name == null
                    orderby i.Date, i.Age
                    select i)
                .Skip(5).Take(3).Select(x => new {x.Id, x.Age});
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(3);
        }
        
        [Fact]
        public void MustSearchWithMainFromFormat5()
        {
            //Given
            var data = Fixture.CreateMany<SampleData>(10).ToList();
            data[2].Name = "9210964 " + Fixture.Create<string>();
            data[3].LastName = Fixture.Create<string>() + " 9210964";
            data[6].Age = 9210964;
            
            Bulk(data);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x =>
                x.Name.Contains("9210964")
                || x.LastName.Contains("9210964")
                || x.Age == 9210964
            ).ToList();

            //Then
            results.Should().HaveCount(3);
            results.Should().ContainSingle(x => x.Name == data[2].Name);
            results.Should().ContainSingle(x => x.LastName == data[3].LastName);
            results.Should().ContainSingle(x => x.Age == data[6].Age);
        }

        [Theory]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\")) && Name == \"Test\"")]
        [InlineData("Name == \"Test\" && Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Age == 846445 && Emails.Any(x => x.Contains(\"test@test.com\")) && Name == \"Test\"")]
        [InlineData("Name == \"Test\" && Emails.Any(x => x.Contains(\"test@test.com\")) && Age == 846445")]
        public void MustSearchInsideListAndOthersFields(string filter)
        {
            //Given
            var data = Fixture.CreateMany<SampleData>(10).ToList();
            data[1].Name = "Test";
            data[1].Age = 846445;
            data[1].Emails[1] = "test@test.com";

            Bulk(data);
            ElasticClient.Indices.Refresh();

            //When
            var byElastic = Sut.Where(filter).OrderBy(x => x.Id).ToList();
            var byMemory = data.AsQueryable().Where(filter).OrderBy(x => x.Id).ToList();

            //Then
            CompareAsJson(byElastic, byMemory);
        }

        [Theory]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\")) || Name == \"Test\"")]
        [InlineData("Name == \"Test\" || Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Age == 846445 || Emails.Any(x => x.Contains(\"test@test.com\")) || Name == \"Test\"")]
        [InlineData("Name == \"Test\" || Emails.Any(x => x.Contains(\"test@test.com\")) || Age == 846445")]
        public void MustSearchInsideListOrOthersFields1(string filter)
        {
            //Given
            var data = Fixture.CreateMany<SampleData>(10).ToList();
            data[1].Name = "Test";
            data[1].Age = 846445;
            data[1].Emails[1] = "test@test.com";

            Bulk(data);
            ElasticClient.Indices.Refresh();

            //When
            var byElastic = Sut.Where(filter).OrderBy(x => x.Id).ToList();
            var byMemory = data.AsQueryable().Where(filter).OrderBy(x => x.Id).ToList();

            //Then
            CompareAsJson(byElastic, byMemory);
        }

        [Theory]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Emails.Any(x => x.Contains(\"test@test.com\")) || Name == \"Test\"")]
        [InlineData("Name == \"Test\" || Emails.Any(x => x.Contains(\"test@test.com\"))")]
        [InlineData("Age == 846445 || Emails.Any(x => x.Contains(\"test@test.com\")) || Name == \"Test\"")]
        [InlineData("Name == \"Test\" || Emails.Any(x => x.Contains(\"test@test.com\")) || Age == 846445")]
        public void MustSearchInsideListOrOthersFields2(string filter)
        {
            //Given
            var data = Fixture.CreateMany<SampleData>(10).ToList();
            data[0].Name = "Test";
            data[1].Age = 846445;
            data[2].Emails[1] = "test@test.com";

            Bulk(data);
            ElasticClient.Indices.Refresh();

            //When
            var byElastic = Sut.Where(filter).OrderBy(x => x.Id).ToList();
            var byMemory = data.AsQueryable().Where(filter).OrderBy(x => x.Id).ToList();

            //Then
            CompareAsJson(byElastic, byMemory);
        }

        [Fact]
        public void WhereWithTwoConditionsUsingContainAndExtrinsicComparison()
        {
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id)
                .ToList();

            var byMemory = items.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id)
                .ToList();
            
            CompareAsJson(byElastic, byMemory);
        }

        [Fact]
        public void WhereWithThreeConditionsUsingContainAndExtrinsicComparisonWithFoundValue()
        {
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(x => x.Name == items[0].Name);
            var byMemory = Sut.Where(x => x.Name == items[0].Name);

            byElastic = byElastic.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byMemory = byMemory.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byElastic.Should().HaveCount(1);
            byElastic.ToList()[0].Id.Should().Be(items[0].Id);
            byMemory.Should().HaveCount(1);
            byMemory.ToList()[0].Id.Should().Be(items[0].Id);
        }
        
        [Fact]
        public void WhereWithThreeConditionsUsingContainAndExtrinsicComparisonWithoutFoundValue()
        {
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(x => x.Name == items[3].Name);
            var byMemory = Sut.Where(x => x.Name == items[3].Name);

            byElastic = byElastic.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byMemory = byMemory.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byElastic.Should().HaveCount(0);
            byMemory.Should().HaveCount(0);
        }
        
        [Fact]
        public void WhereWithFourConditionsUsingContainAndExtrinsicComparison()
        {
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(x => x.Name == items[0].Name);
            var byMemory = Sut.Where(x => x.Name == items[0].Name);

            byElastic = byElastic.Where(item => item.Age == items[0].Age);

            byElastic = byElastic.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byMemory = byMemory.Where(item =>
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId)))
                    || (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value))
                )
                .OrderBy(x => x.Id);

            byElastic.Should().HaveCount(1);
            byElastic.ToList()[0].Id.Should().Be(items[0].Id);
            byMemory.Should().HaveCount(1);
            byMemory.ToList()[0].Id.Should().Be(items[0].Id);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhereWithTwoConditionsUsingContainAndIntrinsicComparison(bool trueOrFalse)
        {
            // Given
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            // When
            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(item =>
                    (item.FolderId == null && allowedTypes.Contains(item.TypeId)) == trueOrFalse ||
                    (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value)) == trueOrFalse
                )
                .OrderBy(x => x.Id)
                .ToList();
            
            var byMemory = items.Where(item =>
                    (item.FolderId == null && allowedTypes.Contains(item.TypeId)) == trueOrFalse ||
                    (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value)) == trueOrFalse
                )
                .OrderBy(x => x.Id)
                .ToList();

            // Then
            CompareAsJson(byElastic, byMemory);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhereWithTwoConditionsUsingContainAndIntrinsicComparison2(bool trueOrFalse)
        {
            // Given
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            // When
            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(item => (item.FolderId == null) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();

            var byMemory = items.Where(item => (item.FolderId == null) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();

            // Then
            CompareAsJson(byElastic, byMemory);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhereWithTwoConditionsUsingContainAndIntrinsicComparison3(bool trueOrFalse)
        {
            // Given
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            // When
            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(item => 
                (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId))) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();

            var byMemory = items.Where(item => 
                    (item.FolderId == null && (false || allowedTypes.Contains(item.TypeId))) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();
            
            // Then
            CompareAsJson(byElastic, byMemory);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhereWithTwoConditionsUsingContainAndIntrinsicComparison4(bool trueOrFalse)
        {
            // Given
            var allowedFolders = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var allowedTypes = new List<Guid>
            {
                Fixture.Create<Guid>(),
                Fixture.Create<Guid>()
            };

            var items = Fixture.CreateMany<SampleData>(4).ToList();
            items[0].FolderId = allowedFolders[0];
            items[0].TypeId = allowedTypes[0];
            items[1].FolderId = allowedFolders[2];
            items[2].FolderId = null;
            items[2].TypeId = allowedTypes[1];
            items[3].FolderId = null;

            // When
            Bulk(items);
            ElasticClient.Indices.Refresh();

            var byElastic = Sut.Where(item =>
                (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value)) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();

            var byMemory = items.Where(item =>
                    (item.FolderId != null && allowedFolders.Contains(item.FolderId.Value)) == trueOrFalse)
                .OrderBy(x => x.Id)
                .ToList();
            
            // Then
            CompareAsJson(byElastic, byMemory);
        }
        
        [Fact]
        public void WhereWithAnyAndContainsExpression()
        {
            // Given
            var samples = Fixture.CreateMany<SampleData>().ToList();
            var searchEmail = samples[1].Emails[1];

            // When
            Bulk(samples);
            ElasticClient.Indices.Refresh();
            
            var byElastic = Sut.Where(x => x.Emails.Any(y => y.Contains(searchEmail)))
                .ToList();
            
            var byMemory = samples.Where(x => x.Emails.Any(y => y.Contains(searchEmail)))
                .ToList();

            // Then
            byElastic.Should().HaveCount(1);
            byElastic[0].Id.Should().Be(samples[1].Id);
            CompareAsJson(byElastic, byMemory);
        }
        
        [Fact]
        public void WhereWithAllClauseAndEqualExpression()
        {
            // Given
            var email = Fixture.Create<string>();
            
            var samples = Fixture.CreateMany<SampleData>().ToList();
            samples[1].Emails[0] = email;
            samples[1].Emails[1] = email;
            samples[1].Emails[2] = email;

            // When
            Bulk(samples);
            ElasticClient.Indices.Refresh();
            
            var byElastic = Sut.Where(x => x.Emails.All(y => y == email))
                .ToList();
            
            var byMemory = samples.Where(x => x.Emails.All(y => y == email))
                .ToList();

            // Then
            CompareAsJson(byElastic, byMemory);
        }
        
        [Fact]
        public void WhereWithAllClauseAndNotEqualExpression()
        {
            // Given
            var email = Fixture.Create<string>();
            
            var samples = Fixture.CreateMany<SampleData>().ToList();
            samples[1].Emails[0] = email;
            samples[1].Emails[1] = email;
            samples[1].Emails[2] = email;

            // When
            Bulk(samples);
            ElasticClient.Indices.Refresh();
            
            var byElastic = Sut.Where(x => x.Emails.All(y => y != email))
                .ToList();
            
            var byMemory = samples.Where(x => x.Emails.All(y => y != email))
                .ToList();

            // Then
            CompareAsJson(byElastic, byMemory);
        }
        
        [Fact]
        public void WhereWithAllClauseAndNotEqualExpression2()
        {
            // Given
            var email = Fixture.Create<string>();
            
            var samples = Fixture.CreateMany<SampleData>().ToList();
            samples[1].Emails[0] = email;

            // When
            Bulk(samples);
            ElasticClient.Indices.Refresh();
            
            var byElastic = Sut.Where(x => x.Emails.All(y => y != email))
                .ToList();
            
            var byMemory = samples.Where(x => x.Emails.All(y => y != email))
                .ToList();

            // Then
            CompareAsJson(byElastic, byMemory);
        }

        [Fact]
        public void ShouldOptimizeBoolQueries()
        {
            // Given
            var samples = Fixture.CreateMany<SampleData>(35).ToList();
            var ids = samples.Select(x => x.Id).ToList();
            
            Bulk(samples);
            ElasticClient.Indices.Refresh();

            // When
            
            // By default, indices.query.bool.max_nested_depth is 20.
            // We need optimize bool queries.
            var results = Sut.Where(x =>
                x.Id == ids[0]
                || x.Id == ids[1]
                || x.Id == ids[2]
                || x.Id == ids[3]
                || x.Id == ids[4]
                || x.Id == ids[5]
                || x.Id == ids[6]
                || x.Id == ids[7]
                || x.Id == ids[8]
                || x.Id == ids[9]
                || x.Id == ids[10]
                || x.Id == ids[11]
                || x.Id == ids[12]
                || x.Id == ids[13]
                || x.Id == ids[14]
                || x.Id == ids[15]
                || x.Id == ids[16]
                || x.Id == ids[17]
                || x.Id == ids[18]
                || x.Id == ids[19]
                || x.Id == ids[20]
                || x.Id == ids[21]
                || x.Id == ids[22]
                || x.Id == ids[23]
                || x.Id == ids[24]
                || x.Id == ids[25]
                || x.Id == ids[26]
                || x.Id == ids[27]
                || x.Id == ids[28]
                || x.Id == ids[29]
            );

            // Then
            results.Should().HaveCount(30);
        }
    }
}