using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Elastic.Transport.Extensions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LinqToElasticSearch
{
    public class ElasticQueryExecutor<TK> : IQueryExecutor
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly Serializer _sourceSerializer;
        private readonly string _dataId;
        private readonly PropertyNameInferrerParser _propertyNameInferrerParser;
        private readonly ElasticGeneratorQueryModelVisitor<TK> _elasticGeneratorQueryModelVisitor;
        private const int ElasticQueryLimit = 10000;

        public ElasticQueryExecutor(ElasticsearchClient elasticClient, string dataId)
        {
            _elasticClient = elasticClient;
            _sourceSerializer = elasticClient.SourceSerializer;
            _dataId = dataId;
            _propertyNameInferrerParser = new PropertyNameInferrerParser(_elasticClient);
            _elasticGeneratorQueryModelVisitor = new ElasticGeneratorQueryModelVisitor<TK>(_propertyNameInferrerParser);
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var queryAggregator = _elasticGeneratorQueryModelVisitor.GenerateElasticQuery<T>(queryModel);

            var documents = _elasticClient.SearchAsync<IDictionary<string, object>>(descriptor =>
            {
                descriptor.Index(_dataId);

                if (queryModel.SelectClause != null &&
                    queryModel.SelectClause.Selector is MemberExpression memberExpression)
                {
                    descriptor.Source(new SourceConfig(new SourceFilter
                    {
                        Includes = _propertyNameInferrerParser.Parser(memberExpression.Member.Name)
                    }));
                }

                if (queryAggregator.Skip != null)
                {
                    descriptor.From(queryAggregator.Skip);
                }
                else
                {
                    descriptor.Size(ElasticQueryLimit);
                }

                if (queryAggregator.Take != null)
                {
                    var take = queryAggregator.Take.Value;
                    var skip = queryAggregator.Skip ?? 0;

                    if (skip + take > ElasticQueryLimit)
                    {
                        var exceedCount = skip + take - ElasticQueryLimit;
                        take -= exceedCount;
                    }

                    descriptor.Size(take);
                }

                if (queryAggregator.Query != null)
                {
                    descriptor.Query(queryAggregator.Query);
                }
                else
                {
                    descriptor.Query(q => q.MatchAll(new MatchAllQuery()));
                }

                if (queryAggregator.OrderByExpressions.Any())
                {
                    var sortOptionsCollection = new List<SortOptions>();
                    foreach (var orderByExpression in queryAggregator.OrderByExpressions)
                    {
                        var property = _propertyNameInferrerParser.Parser(orderByExpression.PropertyName) +
                                       orderByExpression.GetKeywordIfNecessary();
                        sortOptionsCollection.Add(SortOptions.Field(property, new FieldSort
                        {
                            Order = orderByExpression.OrderingDirection == OrderingDirection.Asc
                                ? SortOrder.Asc
                                : SortOrder.Desc
                        }));
                    }

                    descriptor.Sort(sortOptionsCollection);
                }

                if (queryAggregator.GroupByExpressions.Any())
                {
                    descriptor.Aggregations(a =>
                        a.Add("composite", ad =>
                        {
                            var compositeAggregation = new CompositeAggregation
                            {
                                Sources = new List<IDictionary<string, CompositeAggregationSource>>()
                            };
                            queryAggregator.GroupByExpressions.ForEach(gbe =>
                            {
                                var field = _propertyNameInferrerParser.Parser(gbe.ElasticFieldName) +
                                            gbe.GetKeywordIfNecessary();

                                var compositeAggregationSources = new Dictionary<string, CompositeAggregationSource>
                                {
                                    {
                                        $"group_by_{gbe.PropertyName}", new CompositeAggregationSource
                                        {
                                            Terms = new CompositeTermsAggregation
                                            {
                                                Field = field,
                                            }
                                        }
                                    }
                                };
                                compositeAggregation.Sources.Add(compositeAggregationSources);
                            });

                            ad.Aggregations(aa =>
                                aa.Add("data_composite", th => th.TopHits(new TopHitsAggregation())));

                            ad.Composite(compositeAggregation);

                        })
                    );
                }
            }).Result;

            if (queryModel.SelectClause?.Selector is MemberExpression)
            {
                var selectResult = _sourceSerializer.Deserialize<ElasticResultList<T>>(
                    _sourceSerializer.SerializeToBytes(documents.Documents.SelectMany(x => x.Values)));
                selectResult.Skip = queryAggregator.Skip ?? 0;
                selectResult.Take = queryAggregator.Take ?? 0;
                selectResult.Total = documents.Total;
                return selectResult;
            }

            if (queryAggregator.GroupByExpressions.Any())
            {
                var originalGroupingType = queryModel.GetResultType().GenericTypeArguments.First();
                var originalGroupingGenerics = originalGroupingType.GetGenericArguments();
                var originalKeyGenerics = originalGroupingGenerics.First();

                var genericListType = typeof(ElasticResultList<>).MakeGenericType(originalGroupingType);
                var values = (ElasticResultList<T>)Activator.CreateInstance(genericListType);
                values.Skip = queryAggregator.Skip ?? 0;
                values.Take = queryAggregator.Take ?? 0;
                values.Total = documents.Total;

                var composite = documents.Aggregations["composite"] as CompositeAggregate;

                foreach (var bucket in composite.Buckets)
                {
                    var key = GenerateKey(bucket.Key, originalKeyGenerics);
                    var list = ((TopHitsAggregate)bucket.Aggregations["data_composite"]).Hits.Hits.Select(d =>
                        _sourceSerializer.Deserialize<TK>((JsonElement)d.Source)).ToList();

                    var grouping = typeof(Grouping<,>);
                    var groupingGenerics = grouping.MakeGenericType(originalGroupingGenerics);
                    var groupingInstance = Activator.CreateInstance(groupingGenerics, key, list);
                    values.Add(groupingInstance);
                }

                return values;
            }

            var result =
                _sourceSerializer.Deserialize<ElasticResultList<T>>(_sourceSerializer.SerializeToBytes(documents.Documents));
            result.Skip = queryAggregator.Skip ?? 0;
            result.Take = queryAggregator.Take ?? 0;
            result.Total = documents.Total;

            return result;
            //return _sourceSerializer.Deserialize<IEnumerable<T>>(_sourceSerializer.SerializeToBytes(documents.Documents));

        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);

            return returnDefaultWhenEmpty ? sequence.FirstOrDefault() : sequence.First();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var queryAggregator = _elasticGeneratorQueryModelVisitor.GenerateElasticQuery<T>(queryModel);

            foreach (var resultOperator in queryModel.ResultOperators)
            {
                if (resultOperator is CountResultOperator)
                {
                    var result = _elasticClient.CountAsync<T>(descriptor =>
                    {
                        descriptor.Indices(_dataId);

                        if (queryAggregator.Query != null)
                        {
                            descriptor.Query(queryAggregator.Query);
                        }
                    }).Result.Count;

                    if (result > ElasticQueryLimit)
                    {
                        result = ElasticQueryLimit;
                    }

                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        return (T)converter.ConvertFromString(result.ToString());
                    }
                }
            }

            return default(T);
        }

        private dynamic GenerateKey(IReadOnlyDictionary<string, FieldValue> ck, Type keyGenerics)
        {
            if (keyGenerics.IsConstructedGenericType == false)
            {
                if (keyGenerics == typeof(DateTime))
                {
                    var date = ck.Values.First();
                    return FormatDateTimeKey((long)date.Value);
                }
                return ck.Values.First().Value;
            }

            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var entry in ck)
            {
                var key = entry.Key.Replace("group_by_", "");
                var type = keyGenerics.GetProperties().FirstOrDefault(x => x.Name == key)?.PropertyType;

                if (type != null && type == typeof(DateTime))
                {
                    var date = (long)entry.Value.Value;
                    expando[key] = FormatDateTimeKey(date);
                    continue;
                }

                expando[key] = entry.Value.Value;
            }

            return JsonSerializer.Deserialize(JsonSerializer.Serialize(expando), keyGenerics);
        }
        private static DateTime FormatDateTimeKey(long d)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return date.AddMilliseconds(d).ToLocalTime();
        }
    }

    public class Grouping<TKey, TElem> : IGrouping<TKey, TElem>
    {
        public TKey Key { get; }

        private readonly IEnumerable<TElem> _values;

        public Grouping(TKey key, IEnumerable<TElem> values)
        {
            Key = key;
            _values = values;
        }

        public IEnumerator<TElem> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}