// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    using Fabric.Terminology.Domain.Models;

    internal sealed class ValueSetCodeDto
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        public ValueSetCodeDto()
        {
        }

        public ValueSetCodeDto(IValueSetCode code)
        {
            this.ValueSetGUID = code.ValueSetGuid;
            this.CodeGUID = code.CodeGuid;
            this.CodeCD = code.Code;
            this.CodeDSC = code.Name;
            this.CodeSystemGuid = code.CodeSystemGuid;
            this.CodeSystemNM = code.CodeSystemName;
            this.LastModifiedDTS = DateTime.UtcNow;
            this.BindingID = EmptyBinding.BindingID;
            this.BindingNM = EmptyBinding.BindingNM;
            this.LastLoadDTS = EmptyBinding.LastLoadDts;
        }

        public Guid ValueSetGUID { get; set; }

        public Guid? CodeGUID { get; set; }

        public string CodeCD { get; set; }

        public string CodeDSC { get; set; }

        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemNM { get; set; }

        public DateTime LastModifiedDTS { get; set; }

        public int BindingID { get; set; }

        public string BindingNM { get; set; }

        public DateTime LastLoadDTS { get; set; }
    }
}