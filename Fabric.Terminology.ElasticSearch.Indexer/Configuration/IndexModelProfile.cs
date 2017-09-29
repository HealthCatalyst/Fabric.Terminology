namespace Fabric.Terminology.ElasticSearch.Indexer.Configuration
{
	using System;

	using AutoMapper;

	using Fabric.Terminology.Domain.Models;
	using Fabric.Terminology.ElasticSearch.Indexer.Models;

	internal class IndexModelProfile : Profile
    {
	    public IndexModelProfile()
	    {
			CreateMap<ICodeSetCode, CodeSetCodeIndexModel>();
			CreateMap<IValueSetCode, ValueSetCodeIndexModel>();
			CreateMap<IValueSetCodeCount, ValueSetCodeCountIndexModel>();
		    CreateMap<IValueSet, ValueSetIndexModel>()
			    .ForMember(d => d.Id, o => o.MapFrom(s => s.ValueSetGuid))
			    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
	    }
    }
}
