namespace Fabric.Terminology.Domain.Exceptions
{
    using System;

    public class ValueSetNotFoundException : Exception
    {
        public ValueSetNotFoundException(string message)
            : base(message)
        {
        }

        public ValueSetNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}