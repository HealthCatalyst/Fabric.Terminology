namespace Fabric.Terminology.TestsBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

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

                            return Maybe.If(vs.ValueSetCodes.Any(), vs);
                        });
            return mockService;
        }


        public static Mock<IValueSetService> SetupGetValueSets(this Mock<IValueSetService> mockService, IEnumerable<IValueSet> valueSets)
        {
            mockService.Setup(
                    srv => srv.GetValueSets(new string[] { It.IsAny<string>() }, new string[] { It.IsAny<string>() }))
                .Returns(() => valueSets.ToList().AsReadOnly());

            return mockService;
        }

        public static Mock<IValueSetService> SetupAsyncGetValueSets(this Mock<IValueSetService> mockService, PagedCollection<IValueSet> pagedCollection)
        {
            mockService.Setup(
                    srv => srv.GetValueSetsAsync(It.IsAny<IPagerSettings>(), new string[] { It.IsAny<string>() }))
                .ReturnsAsync(() => pagedCollection);

            return mockService;
        }

        public static Mock<IValueSetService> SetupAsyncFindValueSets(this Mock<IValueSetService> mockService, PagedCollection<IValueSet> pagedCollection)
        {
            mockService.Setup(
                    srv => srv.FindValueSetsAsync(
                        It.IsAny<string>(),
                        It.IsAny<IPagerSettings>(),
                        new string[] { It.IsAny<string>() },
                        It.IsAny<bool>()))
                .ReturnsAsync(() => pagedCollection);

            return mockService;
        }
    }
}
