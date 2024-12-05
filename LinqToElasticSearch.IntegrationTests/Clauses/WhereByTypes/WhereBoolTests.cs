using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace LinqToElasticSearch.IntegrationTests.Clauses.WhereByTypes
{
    public class WhereBoolTests : IntegrationTestsBase<SampleData>
    {
        [Fact]
        public void WhereDateEqualImplicit()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Can);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Can.Should().Be(true);
        }

        [Fact]
        public void WhereDateEqualExplicit()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Can == true);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(1);
            listResults[0].Can.Should().Be(true);
        }

        [Fact]
        public void WhereDateEqualNegativeExplicit()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => x.Can != true);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
            listResults[0].Can.Should().Be(false);
            listResults[1].Can.Should().Be(false);
        }

        [Fact]
        public void WhereDateEqualNegativeImplicit()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.Where(x => !x.Can);
            var listResults = results.ToList();

            //Then
            listResults.Count.Should().Be(2);
            listResults[0].Can.Should().Be(false);
            listResults[1].Can.Should().Be(false);
        }

        [Fact]
        public void WhereAndFirstOrDefault()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;
            datas[2].Name = "abcd";

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.FirstOrDefault(x => x.Can==false && x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);
            results.Name.Should().Be("abcd");

            results = Sut.First(x => !x.Can && x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);
            results.Name.Should().Be("abcd");

            results = Sut.FirstOrDefault(x => !x.Can && x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);
            results.Name.Should().Be("abcd");

            results = Sut.FirstOrDefault(x => x.Name == "abcd" && !x.Can );
            //Then
            results.Can.Should().Be(false);
            results.Name.Should().Be("abcd");

            results = Sut.FirstOrDefault(x => x.Name == "abcd" && x.Can);
            //Then
            results.Should().Be(null);
        }

        [Fact]
        public void WhereOrFirstOrDefault()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>().ToList();
            datas[0].Can = false;
            datas[1].Can = true;
            datas[2].Can = false;
            datas[2].Name = "abcd";

            Bulk(datas);
            ElasticClient.Indices.Refresh();

            //When
            var results = Sut.FirstOrDefault(x => x.Can == false || x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);

            results = Sut.First(x => !x.Can || x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);

            results = Sut.FirstOrDefault(x => !x.Can || x.Name == "abcd");
            //Then
            results.Can.Should().Be(false);

            results = Sut.FirstOrDefault(x => x.Name == "abcd" || !x.Can);
            //Then
            results.Can.Should().Be(false);
        }
    }
}