namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class TestObject : IHaveValueSetGuid
    {
        public string Text { get; set; }

        public DateTime Stamp { get; set; }

        public Guid ValueSetGuid { get; internal set; }
    }
}