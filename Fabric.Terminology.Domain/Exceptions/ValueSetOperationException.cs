using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Terminology.Domain.Exceptions
{
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
