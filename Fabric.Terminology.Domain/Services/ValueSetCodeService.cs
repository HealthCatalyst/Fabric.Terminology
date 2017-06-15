using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;

namespace Fabric.Terminology.Domain.Services
{
    public class ValueSetCodeService : TerminologyServiceBase, IValueSetCodeService
    {
        public ValueSetCodeService(IValueSetCodeRepository valueSetCodeRepository) : base(valueSetCodeRepository)
        {
        }

        public IEnumerable<IValueSetCode> GetCodesByValueSet(string valueSetId)
        {
            throw new System.NotImplementedException();
        }

        public PagedCollection<IValueSetCode> GetValueSetCodes(string codeSystemCode, IPagerSettings settings)
        {
            throw new System.NotImplementedException();
        }
    }
}