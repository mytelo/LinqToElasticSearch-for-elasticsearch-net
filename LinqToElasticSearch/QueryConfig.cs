using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;

namespace LinqToElasticSearch
{
    public abstract class QueryConfig
    {
        public Field Field { get; set; }

        /// <summary>
        /// <para>
        /// Text, number, boolean value or date you wish to find in the provided field.
        /// </para>
        /// </summary>
        public object Query { get; set; }
        public string QueryName { get; set; }


        public abstract Query GetQuery();
    }



    public class MatchQueryConfig : QueryConfig
    {
        /// <summary>
        /// <para>
        /// Analyzer used to convert the text in the query value into tokens.
        /// </para>
        /// </summary>
        public string Analyzer { get; set; }

        /// <summary>
        /// <para>
        /// If <c>true</c>, match phrase queries are automatically created for multi-term synonyms.
        /// </para>
        /// </summary>
        public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

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
                Query = Query,
                QueryName = QueryName,
                Analyzer = Analyzer,
                AutoGenerateSynonymsPhraseQuery = AutoGenerateSynonymsPhraseQuery,
                Boost = Boost,
                Field = Field,
                Fuzziness = Fuzziness,
                FuzzyRewrite = FuzzyRewrite,
                FuzzyTranspositions= FuzzyTranspositions,
                Lenient = Lenient,
                MaxExpansions = MaxExpansions,
                MinimumShouldMatch = MinimumShouldMatch,
                Operator = Operator,
                PrefixLength = PrefixLength,
                ZeroTermsQuery = ZeroTermsQuery
            };
        }
    }
}
