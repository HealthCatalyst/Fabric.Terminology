namespace Fabric.Terminology.API.Services
{
    using System;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    public interface IClientTermCustomizationService
    {
        Attempt<IValueSet> CreateValueSet(ClientTermValueSetApiModel model);

        Attempt<IValueSet> UpdateValueSet(Guid valueSetGuid, ClientTermValueSetApiModel model);
    }
}