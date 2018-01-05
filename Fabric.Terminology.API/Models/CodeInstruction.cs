namespace Fabric.Terminology.API.Models
{
    using System;

    public class CodeInstruction
    {
        public Guid IdGuid { get; set; }

        public CodeInstructionSource Source { get; set; } = CodeInstructionSource.CodeSystemCode;

        public CodeInstructionType InstructionType { get; set; } = CodeInstructionType.Add;
    }
}