namespace Fabric.Terminology.API.Models
{
    using System;

    public class CodeOperation
    {
        public Guid Value { get; set; }

        public CodeOperationSource Source { get; set; } = CodeOperationSource.CodeSystemCode;

        public OperationInstruction Instruction { get; set; } = OperationInstruction.Add;
    }
}