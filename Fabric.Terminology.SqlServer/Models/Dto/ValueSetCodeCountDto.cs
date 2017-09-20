// ReSharper disable InconsistentNaming
namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    public class ValueSetCodeCountDto
    {
        public Guid ValueSetGUID { get; set; }

        public Guid CodeSystemGUID { get; set; }

        public int CodeSystemPerValueSetNBR { get; set; }
    }
}