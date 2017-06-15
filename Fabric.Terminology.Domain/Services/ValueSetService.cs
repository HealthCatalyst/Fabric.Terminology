using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;

namespace Fabric.Terminology.Domain.Services
{
    public class ValueSetService : TerminologyServiceBase, IValueSetService
    {
        public ValueSetService(IValueSetCodeRepository valueSetCodeRepository) : base(valueSetCodeRepository)
        {
        }

        public IValueSet Create(string name, IEnumerable<IValueSetCodeItem> valueSetCodes)
        {
            throw new System.NotImplementedException();
        }

        public IValueSet GetValueSet(string valueSetId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IValueSet> GetValueSets(IEnumerable<string> valueSetIds)
        {
            throw new System.NotImplementedException();
        }

        public PagedCollection<IValueSet> GetValueSets(IPagerSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IValueSet valueSet)
        {
            throw new System.NotImplementedException();
        }

        public void Save(IValueSet valueSet)
        {
            throw new System.NotImplementedException();
        }
    }
}