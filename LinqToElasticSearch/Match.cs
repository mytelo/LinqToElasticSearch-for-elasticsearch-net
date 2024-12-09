using Elastic.Clients.Elasticsearch;

namespace LinqToElasticSearch
{
    public class Match
    {
        public static bool MatchQuery(Field field, string keyword, QueryConfig config = null)
        {
            return true;
        }

        public static bool MultiMatchQuery<T>(T t, string keyword, MultiMatchQueryConfig config = null)
        {
            return true;
        }
    }
}
