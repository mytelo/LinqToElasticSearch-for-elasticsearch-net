﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LinqToElasticSearch.JsonConverter;

namespace LinqToElasticSearch.IntegrationTests
{
    public class SampleData
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime Date { get; set; }
        public DateTimeOffset DateOffset { get; set; }

        public DateTime? Date1 { get; set; }
        public DateTimeOffset? DateOffset1 { get; set; }

        //enum to int 
        [JsonConverter(typeof(JsonEnumConverter))]
        public SampleType SampleTypeProperty { get; set; }

        [JsonConverter(typeof(JsonEnumConverter))]
        public SampleType? EnumNullable { get; set; }
        public Guid? FolderId { get; set; }
        public Guid TypeId { get; set; }
        public bool Can { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public TimeSpan? TimeSpanNullable { get; set; }

        public IList<string> Emails { get; set; }


        //enum default string
        public SampleType SampleTypePropertyString { get; set; }
    }

    public enum SampleType
    {
        Sample,
        Type,
        SampleType
    }
}