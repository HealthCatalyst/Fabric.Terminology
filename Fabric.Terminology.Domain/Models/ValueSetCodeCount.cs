namespace Fabric.Terminology.Domain.Models
{
    using System;

    public class ValueSetCodeCount : IValueSetCodeCount
    {
        internal ValueSetCodeCount()
        {
        }

        public Guid ValueSetGuid { get; internal set; }

        public Guid CodeSystemGuid { get; internal set; }

        public int CodeCount { get; internal set; }
    }
}