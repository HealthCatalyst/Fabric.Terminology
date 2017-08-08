namespace Fabric.Terminology.Domain.Exceptions
{
    using System;

    public class ValueSetOperationException : Exception
    {
        public ValueSetOperationException(string message)
            : base(message)
        {
        }

        public ValueSetOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}