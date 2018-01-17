namespace Fabric.Terminology.API
{
    using System;
    using System.Linq;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    public static partial class Extensions
    {
        public static Attempt<IValueSet> Create(this IClientTermValueSetService service, ClientTermValueSetApiModel model)
        {
           // return service.Create(model.Name, model, model.CodeSetCodes.ToList());
            throw new NotImplementedException();
        }
    }
}