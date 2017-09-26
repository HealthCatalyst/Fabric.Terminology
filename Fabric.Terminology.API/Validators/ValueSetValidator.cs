namespace Fabric.Terminology.API.Validators
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using FluentValidation;

    public class ValueSetValidator : AbstractValidator<IValueSet>
    {
        private readonly IValueSetService service;

        public ValueSetValidator(IValueSetService valueSetService)
        {
            this.service = valueSetService;
            this.ConfigureRules();
        }

        private void ConfigureRules()
        {
            //// this.RuleFor(vs => vs.ValueSetId).NotEmpty().WithMessage("Please specify an Id for this client");

            this.RuleFor(vs => vs.Name).Must(this.BeUnique).When(vs => !string.IsNullOrEmpty(vs.Name));

            this.RuleFor(vs => vs.Name).NotEmpty().WithMessage("Please specify a Name for this client");
        }

        private bool BeUnique(string name)
        {
            return this.service.NameIsUnique(name);
        }
    }
}