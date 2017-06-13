using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Services.Persistence
{
    public interface IValueSetCodeRepository
    {
        IEnumerable<IValueSetCode> GetCodesByValueSet(string valueSetId);

        PagedCollection<IValueSetCode> GetValueSetCodes(string codeSystemCode, IPagerSettings settings);

        IEnumerable<IValueSetCode> GetAll();

        IEnumerable<IValueSetCode> GetAll(string codeSystemCode);
    }
}