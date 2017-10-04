namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class ValueSetCode : CodeSetCode, IValueSetCode
    {
        public ValueSetCode()
        {
        }

        internal ValueSetCode(ICodeSetCode codeSetCode)
        {
            this.Code = codeSetCode.Code;
            this.Name = codeSetCode.Name;
            this.CodeSystemGuid = codeSetCode.CodeSystemGuid;
            this.CodeSystemName = codeSetCode.CodeSystemName;
        }

        public Guid ValueSetGuid { get; internal set; }
    }
}