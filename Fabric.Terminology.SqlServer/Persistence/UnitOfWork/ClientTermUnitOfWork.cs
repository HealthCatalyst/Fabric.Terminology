namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Serilog;

    internal class ClientTermUnitOfWork : IClientTermUnitOfWork
    {
        private readonly Lazy<ClientTermContext> context;

        private readonly ILogger logger;

        private Lazy<IUnitOfWorkRepository<ValueSetCodeDto>> valueSetCode;

        private Lazy<IUnitOfWorkRepository<ValueSetCodeCountDto>> valueSetCodeCount;

        private Lazy<IUnitOfWorkRepository<ValueSetDescriptionBaseDto>> valueSetDescription;

        public ClientTermUnitOfWork(Lazy<ClientTermContext> context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
            this.Initialize();
        }

        public IUnitOfWorkRepository<ValueSetDescriptionBaseDto> ValueSetDescriptions => this.valueSetDescription.Value;

        public ClientTermContext Context => this.context.Value;

        public ILogger Logger => this.logger;

        public IUnitOfWorkRepository<ValueSetCodeDto> ValueSetCodes => this.valueSetCode.Value;

        public IUnitOfWorkRepository<ValueSetCodeCountDto> ValueSetCodeCounts => this.valueSetCodeCount.Value;

        public void Commit()
        {
            this.context.Value.SaveChanges();
        }

        private void Initialize()
        {
            this.valueSetDescription = new Lazy<IUnitOfWorkRepository<ValueSetDescriptionBaseDto>>(
                () => new UnitOfWorkRepository<ValueSetDescriptionBaseDto>(this.context.Value, this.logger));

            this.valueSetCode = new Lazy<IUnitOfWorkRepository<ValueSetCodeDto>>(
                () => new UnitOfWorkRepository<ValueSetCodeDto>(this.context.Value, this.logger));

            this.valueSetCodeCount = new Lazy<IUnitOfWorkRepository<ValueSetCodeCountDto>>(
                () => new UnitOfWorkRepository<ValueSetCodeCountDto>(this.context.Value, this.logger));
        }
    }
}