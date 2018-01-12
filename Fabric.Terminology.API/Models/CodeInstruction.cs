namespace Fabric.Terminology.API.Models
{
    using System;

    public class CodeInstruction
    {
        public Guid IdGuid { get; set; }

        public CodeSrc Src { get; set; } = CodeSrc.CodeSystemCode;

        public CodeInstructionType InstructionType { get; set; } = CodeInstructionType.Add;
    }
}