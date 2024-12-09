using System.Linq;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace LinqToElasticSearch
{
    public class NodeVisitor : INodeVisitor
    {
        public Query Visit(BoolNode node)
        {
            var queries = node.Children.Select(x => x.Accept(this));
            return new BoolQuery { Should = queries.ToList() };
        }

        public Query Visit(OrNode node)
        {
            return new BoolNode(node.Optimize()).Accept(this);
        }

        public Query Visit(AndNode node)
        {
            var left = node.Left.Accept(this);
            var right = node.Right.Accept(this);
            return new BoolQuery { Must = new[] { left, right } };
        }

        public Query Visit(NotNode node)
        {
            var child = node.Child.Accept(this);

            return new BoolQuery
            {
                MustNot = new[] { child }
            };
        }

        public Query Visit(TermNode node)
        {
            if (node.Field == null && node.Value is bool v && v == true)
            {
                //match all
                return new MatchAllQuery();
            }

            if (node.Field == null && node.Value is bool v2 && v2 == false)
            {
                //match none
                return new MatchNoneQuery();
            }

            return new TermQuery(node.Field)
            {
                Value = GetFieldValue(node.Value)
            };
        }

        public Query Visit(TermsNode node)
        {
            return new TermsQuery()
            {
                Field = node.Field,
                QueryName = node.Field,
                Terms = new TermsQueryField(node.Values.Select(GetFieldValue).ToList())
            };
        }

        public Query Visit(TermsSetNode node)
        {
            return new TermsSetQuery(node.Field)
            {
                QueryName = node.Field,
                Terms = node.Values.Select(GetFieldValue).ToList(),
                MinimumShouldMatchScript = node.Equal
                    ? new Script
                    {
                        Source = $"doc['{node.Field}'].length"
                    }
                    : new Script
                    {
                        Source = "0"
                    }
            };
        }

        public Query Visit(ExistsNode node)
        {
            return new BoolQuery
            {
                Must = new Query[]
                {
                    new ExistsQuery
                    {
                        Field = node.Field
                    }
                }
            };
        }

        public Query Visit(NotExistsNode node)
        {
            return new BoolQuery
            {
                MustNot = new Query[]
                {
                    new ExistsQuery
                    {
                        Field = node.Field
                    }
                }
            };
        }

        public Query Visit(DateRangeNode node)
        {
            return new DateRangeQuery(node.Field)
            {
                QueryName = node.Field,
                Lt = node.LessThan,
                Lte = node.LessThanOrEqualTo,
                Gt = node.GreaterThan,
                Gte = node.GreaterThanOrEqualTo
            };
        }

        public Query Visit(MatchPhraseNode node)
        {
            return new MatchPhraseQuery(node.Field)
            {
                QueryName = node.Field,
                Query = (string)node.Value
            };
        }

        public Query Visit(NumericRangeNode node)
        {
            return new NumberRangeQuery(node.Field)
            {
                QueryName = node.Field,
                Lt = node.LessThan,
                Lte = node.LessThanOrEqualTo,
                Gt = node.GreaterThan,
                Gte = node.GreaterThanOrEqualTo
            };
        }

        public Query Visit(QueryStringNode node)
        {
            return new QueryStringQuery
            {
                Fields = new[] { node.Field },
                QueryName = node.Field,
                Query = (string)node.Value
            };
        }

        public Query Visit(QueryMatchNode node)
        {
            return node.QueryConfig.GetQuery();
        }

        public Query Visit(MultiMatchNode node)
        {
            return new MultiMatchQuery
            {
                Fields = new[] { node.Field },
                QueryName = node.Field,
                Type = TextQueryType.PhrasePrefix,
                Query = (string)node.Value,
                MaxExpansions = 200
            };
        }

        public Query Visit(MultiMatchQueryNode node)
        {
            return node.MultiMatchQueryConfig.GetQuery();
        }

        private FieldValue GetFieldValue(object value)
        {
            switch (value)
            {
                case string v:
                {
                    return FieldValue.String(v);
                }
                case bool v:
                {
                    return FieldValue.Boolean(v);
                }
                case long v:
                {
                    return FieldValue.Long(v);
                }
                case float v:
                {
                    return FieldValue.Double(v);
                }
                case double v:
                {
                    return FieldValue.Double(v);
                }
                case decimal v:
                {
                    return FieldValue.Double((double)v);
                }
                default :
                    return FieldValue.String(value.ToString());
            }

        }
    }
}