using System;
using System.Linq;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;

namespace LinqToElasticSearch
{
    public abstract class QueryConfig
    {
        public Field Field { get; set; }

        public string QueryName { get; set; }

        /// <summary>
        /// <para>
        /// Text, number, boolean value or date you wish to find in the provided field.
        /// </para>
        /// </summary>
        public object Query { get; set; }

        /// <summary>
        /// <para>
        /// Analyzer used to convert the text in the query value into tokens.
        /// </para>
        /// </summary>
        public string Analyzer { get; set; }

        /// <summary>
        /// <para>
        /// Floating point number used to decrease or increase the relevance scores of the query.
        /// Boost values are relative to the default value of 1.0.
        /// A boost value between 0 and 1.0 decreases the relevance score.
        /// A value greater than 1.0 increases the relevance score.
        /// </para>
        /// </summary>
        public float? Boost { get; set; }

        public abstract Query GetQuery();
    }



    public class MatchQueryConfig : QueryConfig
    {
        /// <summary>
        /// <para>
        /// If <c>true</c>, match phrase queries are automatically created for multi-term synonyms.
        /// </para>
        /// </summary>
        public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

        /// <summary>
        /// <para>
        /// Maximum edit distance allowed for matching.
        /// </para>
        /// </summary>
        public Fuzziness Fuzziness { get; set; }

        /// <summary>
        /// <para>
        /// Method used to rewrite the query.
        /// </para>
        /// </summary>
        public string FuzzyRewrite { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, edits for fuzzy matching include transpositions of two adjacent characters (for example, <c>ab</c> to <c>ba</c>).
        /// </para>
        /// </summary>
        public bool? FuzzyTranspositions { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, format-based errors, such as providing a text query value for a numeric field, are ignored.
        /// </para>
        /// </summary>
        public bool? Lenient { get; set; }

        /// <summary>
        /// <para>
        /// Maximum number of terms to which the query will expand.
        /// </para>
        /// </summary>
        public int? MaxExpansions { get; set; }

        /// <summary>
        /// <para>
        /// Minimum number of clauses that must match for a document to be returned.
        /// </para>
        /// </summary>
        public MinimumShouldMatch MinimumShouldMatch { get; set; }

        /// <summary>
        /// <para>
        /// Boolean logic used to interpret text in the query value.
        /// </para>
        /// </summary>
        public Operator? Operator { get; set; }

        /// <summary>
        /// <para>
        /// Number of beginning characters left unchanged for fuzzy matching.
        /// </para>
        /// </summary>
        public int? PrefixLength { get; set; }

        /// <summary>
        /// <para>
        /// Indicates whether no documents are returned if the <c>analyzer</c> removes all tokens, such as when using a <c>stop</c> filter.
        /// </para>
        /// </summary>
        public ZeroTermsQuery? ZeroTermsQuery { get; set; }


        public override Query GetQuery()
        {
            return new MatchQuery(Field)
            {
                Field = Field,
                Query = Query,
                QueryName = QueryName,
                Analyzer = Analyzer,
                AutoGenerateSynonymsPhraseQuery = AutoGenerateSynonymsPhraseQuery,
                Boost = Boost,
                Fuzziness = Fuzziness,
                FuzzyRewrite = FuzzyRewrite,
                FuzzyTranspositions = FuzzyTranspositions,
                Lenient = Lenient,
                MaxExpansions = MaxExpansions,
                MinimumShouldMatch = MinimumShouldMatch,
                Operator = Operator,
                PrefixLength = PrefixLength,
                ZeroTermsQuery = ZeroTermsQuery
            };
        }
    }

    public class MatchPhraseQueryConfig : QueryConfig
    {
        /// <summary>
        /// <para>
        /// Maximum number of positions allowed between matching tokens.
        /// </para>
        /// </summary>
        public int? Slop { get; set; }

        /// <summary>
        /// <para>
        /// Indicates whether no documents are returned if the <c>analyzer</c> removes all tokens, such as when using a <c>stop</c> filter.
        /// </para>
        /// </summary>
        public ZeroTermsQuery? ZeroTermsQuery { get; set; }


        public override Query GetQuery()
        {
            return new MatchPhraseQuery(Field)
            {
                Field = Field,
                Query = Query.ToString(),
                QueryName = QueryName,
                Analyzer = Analyzer,
                Boost = Boost,
                Slop = Slop,
                ZeroTermsQuery = ZeroTermsQuery
            };
        }
    }

    public class MatchPhrasePrefixQueryConfig : QueryConfig
    {
        /// <summary>
        /// <para>
        /// Maximum number of positions allowed between matching tokens.
        /// </para>
        /// </summary>
        public int? Slop { get; set; }

        /// <summary>
        /// <para>
        /// Maximum number of terms to which the last provided term of the query value will expand.
        /// </para>
        /// </summary>
        public int? MaxExpansions { get; set; }

        /// <summary>
        /// <para>
        /// Indicates whether no documents are returned if the <c>analyzer</c> removes all tokens, such as when using a <c>stop</c> filter.
        /// </para>
        /// </summary>
        public ZeroTermsQuery? ZeroTermsQuery { get; set; }


        public override Query GetQuery()
        {
            return new MatchPhrasePrefixQuery(Field)
            {
                Field = Field,
                Query = Query.ToString(),
                QueryName = QueryName,
                Analyzer = Analyzer,
                Boost = Boost,
                Slop = Slop,
                MaxExpansions = MaxExpansions,
                ZeroTermsQuery = ZeroTermsQuery
            };
        }
    }

    public class MultiMatchQueryConfig
    {
        public string QueryName { get; set; }

        /// <summary>
        /// <para>
        /// Text, number, boolean value or date you wish to find in the provided field.
        /// </para>
        /// </summary>
        public object Query { get; set; }

        /// <summary>
        /// <para>
        /// Analyzer used to convert the text in the query value into tokens.
        /// </para>
        /// </summary>
        public string Analyzer { get; set; }

        /// <summary>
        /// <para>
        /// Floating point number used to decrease or increase the relevance scores of the query.
        /// Boost values are relative to the default value of 1.0.
        /// A boost value between 0 and 1.0 decreases the relevance score.
        /// A value greater than 1.0 increases the relevance score.
        /// </para>
        /// </summary>
        public float? Boost { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, match phrase queries are automatically created for multi-term synonyms.
        /// </para>
        /// </summary>
        public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

        /// <summary>
        /// <para>
        /// The fields to be queried.
        /// Defaults to the <c>index.query.default_field</c> index settings, which in turn defaults to <c>*</c>.
        /// </para>
        /// </summary>
        public Fields Fields { get; set; }

        /// <summary>
        /// <para>
        /// Maximum edit distance allowed for matching.
        /// </para>
        /// </summary>
        public Fuzziness Fuzziness { get; set; }

        /// <summary>
        /// <para>
        /// Method used to rewrite the query.
        /// </para>
        /// </summary>
        public string FuzzyRewrite { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, edits for fuzzy matching include transpositions of two adjacent characters (for example, <c>ab</c> to <c>ba</c>).
        /// Can be applied to the term subqueries constructed for all terms but the final term.
        /// </para>
        /// </summary>
        public bool? FuzzyTranspositions { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, format-based errors, such as providing a text query value for a numeric field, are ignored.
        /// </para>
        /// </summary>
        public bool? Lenient { get; set; }

        /// <summary>
        /// <para>
        /// Maximum number of terms to which the query will expand.
        /// </para>
        /// </summary>
        public int? MaxExpansions { get; set; }

        /// <summary>
        /// <para>
        /// Minimum number of clauses that must match for a document to be returned.
        /// </para>
        /// </summary>
        public MinimumShouldMatch MinimumShouldMatch { get; set; }

        /// <summary>
        /// <para>
        /// Boolean logic used to interpret text in the query value.
        /// </para>
        /// </summary>
        public Operator? Operator { get; set; }

        /// <summary>
        /// <para>
        /// Number of beginning characters left unchanged for fuzzy matching.
        /// </para>
        /// </summary>
        public int? PrefixLength { get; set; }

        /// <summary>
        /// <para>
        /// Maximum number of positions allowed between matching tokens.
        /// </para>
        /// </summary>
        public int? Slop { get; set; }

        /// <summary>
        /// <para>
        /// Determines how scores for each per-term blended query and scores across groups are combined.
        /// </para>
        /// </summary>
        public double? TieBreaker { get; set; }

        /// <summary>
        /// <para>
        /// How <c>the</c> multi_match query is executed internally.
        /// </para>
        /// </summary>
        public TextQueryType? Type { get; set; }

        /// <summary>
        /// <para>
        /// Indicates whether no documents are returned if the <c>analyzer</c> removes all tokens, such as when using a <c>stop</c> filter.
        /// </para>
        /// </summary>
        public ZeroTermsQuery? ZeroTermsQuery { get; set; }


        public Query GetQuery()
        {
            if (Fields == null || !Fields.Any())
            {
                throw new Exception("Fields must not be empty");
            }

            return new MultiMatchQuery
            {
                Fields = Fields,
                AutoGenerateSynonymsPhraseQuery = AutoGenerateSynonymsPhraseQuery,
                Fuzziness = Fuzziness,
                FuzzyRewrite = FuzzyRewrite,
                FuzzyTranspositions= FuzzyTranspositions,
                Lenient = Lenient,
                Query = Query.ToString(),
                QueryName = QueryName,
                Analyzer = Analyzer,
                Boost = Boost,
                Slop = Slop,
                MaxExpansions = MaxExpansions,
                MinimumShouldMatch = MinimumShouldMatch,
                Operator = Operator,
                PrefixLength = PrefixLength,
                TieBreaker = TieBreaker,
                Type = Type,
                ZeroTermsQuery = ZeroTermsQuery
            };
        }
    }

    //Others
}
