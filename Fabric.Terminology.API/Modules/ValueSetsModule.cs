using System;
using Fabric.Terminology.Domain.Services;
using Nancy;

namespace Fabric.Terminology.API.Modules
{
    public sealed class ValueSetsModule : NancyModule
    {
        public ValueSetsModule(IValueSetService valueSetService)
            : base("/api/valuesets")
        {
            this.ValueSetService = valueSetService ?? throw new ArgumentNullException(nameof(valueSetService));

            Get("/", arg => "Gets value sets");
        }

        private IValueSetService ValueSetService { get; }
    }
}