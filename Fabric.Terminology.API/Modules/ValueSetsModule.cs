namespace Fabric.Terminology.API.Modules
{
    using System;
    using Fabric.Terminology.Domain.Services;
    using Nancy;

    public sealed class ValueSetsModule : NancyModule
    {
        public ValueSetsModule(IValueSetService valueSetService)
            : base("/api/valuesets")
        {
            this.ValueSetService = valueSetService ?? throw new ArgumentNullException(nameof(valueSetService));

            this.Get("/", arg => "Gets value sets");
            this.Get("/summary/{valueSetId}", args => "test");

            this.Get("/summaries/", args => "something");

            this.Post("/find/", _ => "something");
        }

        private IValueSetService ValueSetService { get; }

    }
}