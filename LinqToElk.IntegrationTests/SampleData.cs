﻿using System;

namespace LinqToElk.IntegrationTests
{
    public class SampleData
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public DateTime Date { get; set; }
        
        public SampleType SampleTypeProperty { get; set; }
        
        public bool Can { get; set; }
    }

    public enum SampleType
    {
        Sample,
        Type,
        SampleType
    }
}