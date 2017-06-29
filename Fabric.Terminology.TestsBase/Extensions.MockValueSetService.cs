namespace Fabric.Terminology.TestsBase
{
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Moq;

    public static partial class Extensions
    {
        public static Mock<IValueSetService> SetupGetValueSet(this Mock<IValueSetService> mockService, IEnumerable<IValueSet> valueSets)
        {
            mockService.Setup(srv => srv.GetValueSet(It.IsAny<string>(), new string[] { It.IsAny<string>() }))
                .Returns(
                    (string valueSetId, string[] codesystems) =>
                        {
                            var vs = valueSets.FirstOrDefault(x => x.ValueSetId == valueSetId);
                            if (codesystems.Any())
                            {
                                ((ValueSet)vs).ValueSetCodes = vs.ValueSetCodes
                                    .Where(code => codesystems.Contains(code.CodeSystem.Code))
                                    .Select(code => code)
                                    .ToList()
                                    .AsReadOnly();

                            }

                            return vs.ValueSetCodes.Any() ? vs : null;
                        });

            return mockService;
        }
    }
}
