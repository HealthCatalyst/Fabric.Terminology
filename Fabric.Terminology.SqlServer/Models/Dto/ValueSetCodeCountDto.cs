// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetCodeCountDto
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        public ValueSetCodeCountDto()
        {
        }

        public ValueSetCodeCountDto(IValueSetCodeCount count)
            : this(count.ValueSetGuid, count.CodeSystemGuid, count.CodeSystemName, count.CodeCount)
        {
        }

        public ValueSetCodeCountDto(Guid valueSetGuid, Guid codeSystemGuid, string codeSystemName, int codeCount)
        {
            this.ValueSetGUID = valueSetGuid;
            this.CodeSystemGUID = codeSystemGuid;
            this.CodeSystemNM = codeSystemName;
            this.CodeSystemPerValueSetNBR = codeCount;
            this.BindingID = EmptyBinding.BindingID;
            this.BindingNM = EmptyBinding.BindingNM;
            this.LastLoadDTS = EmptyBinding.LastLoadDts;
        }

        public Guid ValueSetGUID { get; set; }

        public Guid CodeSystemGUID { get; set; }

        public string CodeSystemNM { get; set; }

        public int CodeSystemPerValueSetNBR { get; set; }

        public int BindingID { get; set; }

        public string BindingNM { get; set; }

        public DateTime LastLoadDTS { get; set; }
    }
}