namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Serilog;

    internal partial class ClientTermValueUnitOfWorkManager : IClientTermValueUnitOfWorkManager
    {
        private readonly Lazy<ClientTermContext> context;

        private readonly ILogger logger;

        private Lazy<ICrudRepository<ValueSetCodeDto>> valueSetCode;

        private Lazy<ICrudRepository<ValueSetCodeCountDto>> valueSetCodeCount;

        private Lazy<ICrudRepository<ValueSetDescriptionBaseDto>> valueSetDescription;

        public ClientTermValueUnitOfWorkManager(
            Lazy<ClientTermContext> context,
            ILogger logger)
        {
            this.context = context;
            this.logger = logger;
            this.Initialize();
        }

        public ClientTermContext Context => this.context.Value;

        internal ICrudRepository<ValueSetDescriptionBaseDto> ValueSetDescriptions => this.valueSetDescription.Value;

        internal ICrudRepository<ValueSetCodeDto> ValueSetCodes => this.valueSetCode.Value;

        internal ICrudRepository<ValueSetCodeCountDto> ValueSetCodeCounts => this.valueSetCodeCount.Value;

        public Maybe<ValueSetDescriptionBaseDto> GetValueSetDescriptionDto(Guid valueSetGuid) =>
            Maybe.From(this.ValueSetDescriptions.SingleOrDefault(vsd => vsd.ValueSetGUID == valueSetGuid));

        public IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid) =>
            this.ValueSetCodes.Get(vsc => vsc.ValueSetGUID == valueSetGuid);

        public IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid) =>
            this.ValueSetCodeCounts.Get(vscc => vscc.ValueSetGUID == valueSetGuid);

        private void Initialize()
        {
            this.valueSetDescription = new Lazy<ICrudRepository<ValueSetDescriptionBaseDto>>(
                () => new SqlCrudRepository<ValueSetDescriptionBaseDto>(this.context.Value, this.logger));

            this.valueSetCode = new Lazy<ICrudRepository<ValueSetCodeDto>>(
                () => new SqlCrudRepository<ValueSetCodeDto>(this.context.Value, this.logger));

            this.valueSetCodeCount = new Lazy<ICrudRepository<ValueSetCodeCountDto>>(
                () => new SqlCrudRepository<ValueSetCodeCountDto>(this.context.Value, this.logger));
        }
    }
}