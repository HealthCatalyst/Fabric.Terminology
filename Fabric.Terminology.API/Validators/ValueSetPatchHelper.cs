namespace Fabric.Terminology.API.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;

    internal static class ValueSetPatchHelper
    {
        public static Attempt<ClientTermValueSetApiModel> ValidatePatchModel(Guid valueSetGuid, ClientTermValueSetApiModel model)
        {
            if (AttemptsToUseItselfAsCodeSource(valueSetGuid, model.CodeOperations))
            {
                return FailedAttempt(
                    "Invalid model. Operation would result in an attempt to add value set to itself. ValueSetGuid references the value set to which the operation would be applied.",
                    model);
            }

            if (AttemptsToPerformOperationOnDuplicateReferenceKey(model.CodeOperations, CodeOperationSource.ValueSet))
            {
                return FailedAttempt("Invalid model.  Found a duplicate valueSetGuid in codeOperation values.", model);
            }

            if (AttemptsToPerformOperationOnDuplicateReferenceKey(
                model.CodeOperations,
                CodeOperationSource.CodeSystemCode))
            {
                return FailedAttempt(
                    "Invalid model.  Found a duplicate codeGuid in codeOperation values.  Codes must be unique in a value set and it ambiguous to attempt to both add and remove the same code.",
                    model);
            }

            return Attempt<ClientTermValueSetApiModel>.Successful(model);
        }

        private static Attempt<ClientTermValueSetApiModel> FailedAttempt(string msg, ClientTermValueSetApiModel model) =>
            Attempt<ClientTermValueSetApiModel>.Failed(new ValueSetOperationException(msg), model);

        // Ensure that there are not code operations that reference value set to be patched
        private static bool AttemptsToUseItselfAsCodeSource(Guid valueSetGuid, IEnumerable<CodeOperation> codeOperations) =>
            codeOperations.Any(co => co.Source == CodeOperationSource.ValueSet && co.Value == valueSetGuid);

        private static bool AttemptsToPerformOperationOnDuplicateReferenceKey(IEnumerable<CodeOperation> codeOperations, CodeOperationSource source)
        {
            var operations = codeOperations.Where(co => co.Source == source).ToList();
            return operations.Distinct().Count() == operations.Count();
        }
    }
}
