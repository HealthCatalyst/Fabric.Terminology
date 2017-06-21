using System;
using Fabric.Terminology.Domain.Persistence;

namespace Fabric.Terminology.Domain.Services
{
    public abstract class TerminologyServiceBase
    {
        protected TerminologyServiceBase(IValueSetCodeRepository valueSetCodeRepository)
        {
            ValueSetCodeRepository = valueSetCodeRepository ?? throw new ArgumentNullException(nameof(valueSetCodeRepository));
        }

        protected IValueSetCodeRepository ValueSetCodeRepository { get; }
    }
}