namespace Fabric.Terminology.ElasticSearch.Configuration
{
    using AutoMapper;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.ElasticSearch.Models;

    internal class IndexModelProfile : Profile
    {
        public IndexModelProfile()
        {
            this.CreateMap<ICodeSystemCode, CodeSystemCodeIndexModel>();
            this.CreateMap<IValueSetCode, ValueSetCodeIndexModel>();
            this.CreateMap<IValueSetCodeCount, ValueSetCodeCountIndexModel>();
            this.CreateMap<IValueSet, ValueSetIndexModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.ValueSetGuid.ToString()));
        }
    }
}