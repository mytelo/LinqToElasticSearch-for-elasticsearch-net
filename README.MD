# LINQ To ElasticSearch

> 🚧 Experimental State 🚧

This repository is a LINQ Provider To ElasticSearch


# Compatibility
This Library has compatibility with .NET Client v8 and ElasticSearch 8.x version.

# Support Clauses with Integration Tests
* Select
* Where
  * Where(x => x.Any(single expression))
  * Where(x => x.All(single expression))
  * Where(x => x.Contains(string))
  * Where(x => x.StartsWith(string))
  * Where(x => x.EndsWith(string))
* OrderBy
* Skip
* Take
* GroupBy(x => x.Prop1)
* GroupBy(x => new {x.Prop1, x.Prop2})
