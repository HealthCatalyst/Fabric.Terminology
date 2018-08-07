namespace Fabric.Terminology.API.Bootstrapping.MapperProfiles
{
    using AutoMapper;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    internal class CodeSystemApiProfile : Profile
    {
        public CodeSystemApiProfile()
        {
            this.CreateMap<ICodeSystem, CodeSystemApiModel>();
        }
    }
}