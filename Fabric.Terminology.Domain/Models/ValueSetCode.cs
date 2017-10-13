namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class ValueSetCode : CodeSystemCode, IValueSetCode
    {
        public ValueSetCode()
        {
        }

        internal ValueSetCode(ICodeSystemCode codeSystemCode)
        {
            this.Code = codeSystemCode.Code;
            this.Name = codeSystemCode.Name;
            this.CodeSystemGuid = codeSystemCode.CodeSystemGuid;
            this.CodeSystemName = codeSystemCode.CodeSystemName;
        }

        public Guid ValueSetGuid { get; internal set; }
    }
}