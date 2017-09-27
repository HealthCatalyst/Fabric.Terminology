namespace Fabric.Terminology.API.Models
{
    using System;

    public class ValueSetCodeCountApiModel
    {
        public string ValueSetGuid { get; internal set; }

        public Guid CodeSystemGuid { get; internal set; }

        public int CodeCount { get; internal set; }
    }
}