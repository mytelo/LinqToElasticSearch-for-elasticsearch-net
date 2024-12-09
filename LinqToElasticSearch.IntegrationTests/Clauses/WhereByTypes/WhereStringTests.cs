using System.Linq;
using AutoFixture;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FluentAssertions;
using Xunit;

namespace LinqToElasticSearch.IntegrationTests.Clauses.WhereByTypes
{
    public class WhereStringTests : IntegrationTestsBase<SampleData>
    {
        [Fact]
        public void WhereStringEqual()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "abcdef";
            datas[1].Name = "abcdefg";
            datas[2].Name = "abcdef ghi";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name == "abcdef");
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[0].Name);
        }

        [Fact]
        public void WhereMatchQuery()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "【市场价2532】HUAWEI WATCH 2 Pro 4G华为智能的手表 移动支付";
            datas[1].Name = "【市场价8199】HUAWEI Pura 70 Pro 5G华为智能的手机 移动智慧生活";
            datas[2].Name = "abcdef ghi";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => Match.MatchQuery(x.Name, "华为智能的手表和手机", new MatchQueryConfig
            {
                Analyzer = "ik_smart"
            }));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
            listResults[0].Name.Should().Be(datas[0].Name);
            listResults[1].Name.Should().Be(datas[1].Name);
        }

        [Fact]
        public void WhereMatchPhraseQuery()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "【市场价2532】HUAWEI WATCH 2 Pro 4G华为智能的移动支付手表";
            datas[1].Name = "【市场价8199】HUAWEI Pura 70 Pro 5G华为智能的移动智慧生活手机";
            datas[2].Name = "abcdef ghi";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => Match.MatchQuery(x.Name, "华为智能智慧手机", new MatchPhraseQueryConfig
            {
                Analyzer = "ik_smart",
                Slop = 3 //华为智能[的][移动]智慧[生活]手机
            }));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }



        [Fact]
        public void WhereMatchPhrasePrefixQuery()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "华为智能的手表";
            datas[1].Name = "华为智能的手机";
            datas[2].Name = "abcdef ghi";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => Match.MatchQuery(x.Name, "华为智能的手", new MatchPhrasePrefixQueryConfig
            {
                Analyzer = "ik_smart",
                MaxExpansions = 2 //手[表/机]
            }));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
        }

        [Fact]
        public void WhereMultiMatchQuery()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "华为智能的手表";
            datas[0].LastName = "移动支付手表";
            datas[1].Name = "小米手机";
            datas[1].LastName = "小米移动支付手机,智慧生活";
            datas[2].Name = "abcdef ghi";
            datas[2].LastName = "abcdef ghi";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => Match.MultiMatchQuery(x, "智能移动支付", new MultiMatchQueryConfig
            {
                Fields = new[]
                {
                    "name^2", //Can't be set to x.Name, not supported
                    "lastName"
                },
                Analyzer = "ik_smart",
                Operator = Operator.Or
            }));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
        }

        [Fact]
        public void WhereStringNotEqual()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name != datas[1].Name);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
            listResults[0].Name.Should().Be(datas[0].Name);
            listResults[1].Name.Should().Be(datas[2].Name);
        }

        [Fact]
        public void WhereStringContains()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(2).ToList();

            datas[0].Name = "abd";
            datas[1].Name = "abcdefgh";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name.Contains("abc"));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }

        [Fact]
        public void WhereStringContainsWithManyWords()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "Felipe Carlos Ribeiro Cardozo";
            datas[1].Name = "Felipe Cardozo";
            datas[2].Name = "Joao Felipe";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name.Contains("Felipe Cardozo"));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }

        [Fact]
        public void WhereStringContainsToLower()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[1].Name = "abcdefgh";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name.Contains("DefG".ToLower()));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }

        [Fact]
        public void WhereStringNotContains()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "xxxxxxxx";
            datas[1].Name = "abcdefgh";
            datas[2].Name = "yyyyyyyy";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => !x.Name.Contains("defg"));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
            listResults[0].Name.Should().Be(datas[0].Name);
            listResults[1].Name.Should().Be(datas[2].Name);
        }

        [Fact]
        public void WhereStringStartWith()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "0123456789";
            datas[1].Name = "123456789";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name.StartsWith("1234"));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }

        [Fact]
        public void WhereStringEndWith()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();

            datas[0].Name = "123456789";
            datas[1].Name = "12345678";

            Bulk(datas);

            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Name.EndsWith("5678"));
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Name.Should().Be(datas[1].Name);
        }
    }
}