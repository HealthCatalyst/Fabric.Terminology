using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.Domain.Services
{
    public interface IValueSetCodeService
    {
        IEnumerable<IValueSetCode> GetCodesByValueSet(string valueSetId);
        
        PagedCollection<IValueSetCode> GetValueSetCodes(string codeSystemCode, IPagerSettings settings);
    }
}