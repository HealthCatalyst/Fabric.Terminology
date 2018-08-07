namespace Fabric.Terminology.API.Bootstrapping.MapperProfiles
{
    using AutoMapper;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    internal class CodeSystemCodeApiProfile : Profile
    {
        public CodeSystemCodeApiProfile()
        {
            this.CreateMap<ICodeSystemCode, CodeSystemCodeApiModel>();
            this.CreateMap<IBatchCodeSystemCodeResult, BatchCodeResultApiModel>();
        }
    }
}