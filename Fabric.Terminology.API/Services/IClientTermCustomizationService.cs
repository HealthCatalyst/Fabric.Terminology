using Fabric.Terminology.API.Models;
using Fabric.Terminology.Domain;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.API.Services
{
    using System;

    public interface IClientTermCustomizationService
    {
        Attempt<IValueSet> CreateValueSet(ClientTermValueSetApiModel model);

        Attempt<IValueSet> UpdateValueSet(Guid valueSetGuid, ClientTermValueSetApiModel model);
    }
}