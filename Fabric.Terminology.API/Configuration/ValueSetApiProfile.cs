namespace Fabric.Terminology.API.Configuration
{
    using System;

    using AutoMapper;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    internal class ValueSetApiProfile : Profile
    {
        public ValueSetApiProfile()
        {
            this.CreateMap<ICodeSystemCode, CodeSystemCodeApiModel>();
            this.CreateMap<IValueSetCode, ValueSetCodeApiModel>();
            this.CreateMap<IValueSetCodeCount, ValueSetCodeCountApiModel>();
            this.CreateMap<IValueSetSummary, ValueSetItemApiModel>()
                .ForMember(
                    dest => dest.Identifier,
                    opt => opt.MapFrom(
                        src => src.ValueSetGuid.Equals(Guid.Empty)
                                   ? Guid.NewGuid().ToString()
                                   : src.ValueSetGuid.ToString()));

            this.CreateMap<IValueSet, ValueSetApiModel>()
                .ForMember(
                    dest => dest.Identifier,
                    opt => opt.MapFrom(
                        src => src.ValueSetGuid.Equals(Guid.Empty)
                                   ? Guid.NewGuid().ToString()
                                   : src.ValueSetGuid.ToString()));
        }
    }
}