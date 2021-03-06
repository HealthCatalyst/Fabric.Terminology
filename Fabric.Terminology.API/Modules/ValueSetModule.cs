namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Services;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;

    using Serilog;

    public sealed class ValueSetModule : TerminologyModule<ValueSetApiModel>
    {
        private readonly IClientTermValueSetService clientTermValueSetService;

        private readonly IClientTermCustomizationService clientTermCustomizationService;

        private readonly IValueSetComparisonService valueSetComparisonService;

        private readonly IValueSetService valueSetService;

        private readonly IValueSetSummaryService valueSetSummaryService;

        private readonly ValueSetValidatorCollection valueSetValidatorCollection;

        public ValueSetModule(
            IValueSetService valueSetService,
            IValueSetSummaryService valueSetSummaryService,
            IClientTermValueSetService clientTermValueSetService,
            IClientTermCustomizationService clientTermCustomizationService,
            IValueSetComparisonService valueSetComparisonService,
            IAppConfiguration config,
            ILogger logger,
            ValueSetValidatorCollection valueSetValidatorCollection,
            UserAccessService userAccessService)
            : base($"/{TerminologyVersion.Route}/valuesets", config, logger, userAccessService)
        {
            this.valueSetService = valueSetService;
            this.valueSetSummaryService = valueSetSummaryService;
            this.clientTermValueSetService = clientTermValueSetService;
            this.clientTermCustomizationService = clientTermCustomizationService;
            this.valueSetComparisonService = valueSetComparisonService;
            this.valueSetValidatorCollection = valueSetValidatorCollection;

            this.Get("/", async _ => await this.GetValueSetPageAsync().ConfigureAwait(false), null, "GetPaged");

            this.Get("/{valueSetGuid}", parameters => this.GetValueSet(parameters.valueSetGuid), null, "GetValueSet");

            this.Get(
                "/versions/{referenceId}",
                parameters => this.GetValueSetVersions(parameters.referenceId),
                null,
                "GetValueSetVersions");

            this.Post("/multiple/", _ => this.GetMultipleValueSets(), null, "GetMultipleValueSets");

            this.Post("/search/", async _ => await this.SearchAsync().ConfigureAwait(false), null, "Search");

            this.Post("/copy/", _ => this.CopyValueSet(), null, "CopyValueSet");

            this.Post("/compare/", async _ => await this.CompareValueSetsAsync().ConfigureAwait(false), null, "CompareValueSets");

            this.Post("/", _ => this.AddValueSet(), null, "AddValueSet");

            this.Patch("/{valueSetGuid}", parameters => this.PatchValueSet(parameters.valueSetGuid), null, "PatchValueSet");

            this.Put(
                "/{valueSetGuid}/statuscode/{statusCode}",
                parameters => this.ChangeStatus(parameters.valueSetGuid, parameters.statusCode),
                null,
                "ChangeValueSetStatus");

            this.Delete("/{valueSetGuid}", parameters => this.DeleteValueSet(parameters.valueSetGuid), null, "DeleteValueSet");
        }

        private static ValueSetApiModel MapToValueSetApiModel(IValueSet vs, IReadOnlyCollection<Guid> codeSystemGuids) =>
            vs.ToValueSetApiModel(codeSystemGuids);

        private static ValueSetItemApiModel MapToValueSetItemApiModel(
            IValueSetSummary vss,
            IReadOnlyCollection<Guid> codeSystemGuids) =>
            vss.ToValueSetItemApiModel(codeSystemGuids);

        private static async Task<T> ExecuteAsync<T>(Func<Task<T>> query) =>
            await query.Invoke().ConfigureAwait(false);

        private static MultipleValueSetsQuery EnsureQueryModel(MultipleValueSetsQuery model)
        {
            if (model.ValueSetGuids == null)
            {
                model.ValueSetGuids = Array.Empty<Guid>();
            }

            if (model.CodeSystemGuids == null)
            {
                model.CodeSystemGuids = Array.Empty<Guid>();
            }

            return model;
        }

        private object GetValueSet(string valueSetGuidString)
        {
            this.RequireAccessorAuthorization();
            return this.ParseValueSetGuidAndExecute(valueSetGuidString, GetValueSetByGuid);

            object GetValueSetByGuid(Guid valueSetGuid)
            {
                var codeSystemGuids = this.GetCodeSystems();
                var summary = this.GetSummarySetting();

                try
                {
                    var model = summary
                                    ? this.valueSetSummaryService.GetValueSetSummary(valueSetGuid, codeSystemGuids)
                                        .Select(vs => (object)vs.ToValueSetItemApiModel(codeSystemGuids))
                                    : this.valueSetService.GetValueSet(valueSetGuid, codeSystemGuids)
                                        .Select(vs => (object)vs.ToValueSetApiModel(codeSystemGuids));

                    return model.Else(
                        () => this.CreateFailureResponse(
                            "ValueSet with matching ID was not found",
                            HttpStatusCode.NotFound));
                }
                catch (ValueSetNotFoundException ex)
                {
                    this.Logger.Error(ex, "{Message} - ValueSetGuid was {ValueSetGuid}", ex.Message, valueSetGuid);
                    return this.CreateFailureResponse(
                        $"The ValueSet with id: {valueSetGuid} was not found.",
                        HttpStatusCode.InternalServerError);
                }
            }
        }

        private object GetMultipleValueSets()
        {
            this.RequireAccessorAuthorization();
            try
            {
                var model = EnsureQueryModel(this.Bind<MultipleValueSetsQuery>(new BindingConfig { BodyOnly = true }));

                if (!model.ValueSetGuids.Any())
                {
                    return this.CreateFailureResponse(
                        "An array of value set ids is required.",
                        HttpStatusCode.BadRequest);
                }

                return model.Summary
                           ? ExecuteAsync(
                                   () => this.valueSetSummaryService.GetValueSetSummariesListAsync(
                                       model.ValueSetGuids,
                                       model.CodeSystemGuids))
                               .Result.Select(vss => vss.ToValueSetItemApiModel(model.CodeSystemGuids.ToArray()))
                               .ToList()
                           : (IReadOnlyCollection<object>)ExecuteAsync(
                                   () => this.valueSetService.GetValueSetsListAsync(
                                       model.ValueSetGuids,
                                       model.CodeSystemGuids))
                               .Result.Select(vs => vs.ToValueSetApiModel(model.CodeSystemGuids.ToArray()))
                               .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse("Failed to retrieve value sets", HttpStatusCode.InternalServerError);
            }
        }

        private object GetValueSetVersions(string valueSetReferenceId)
        {
            this.RequireAccessorAuthorization();
            try
            {
                var codeSystemGuids = this.GetCodeSystems();
                var summary = this.GetSummarySetting();

                var versions = summary
                                   ? ExecuteAsync(
                                           () => this.valueSetSummaryService.GetValueSetVersionsAsync(
                                               valueSetReferenceId))
                                       .Result.Select(vss => vss.ToValueSetItemApiModel(codeSystemGuids))
                                   : ExecuteAsync(
                                           () => this.valueSetSummaryService.GetValueSetVersionsAsync(
                                               valueSetReferenceId))
                                       .Result.Select(vs => vs.ToValueSetItemApiModel(codeSystemGuids))
                                       .ToList();

                if (!versions.Any())
                {
                    return this.CreateFailureResponse(
                        "ValueSet with matching ValueSetReferenceID was not found",
                        HttpStatusCode.NotFound);
                }

                return versions;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse("Failed to retrieve value sets", HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetValueSetPageAsync()
        {
            this.RequireAccessorAuthorization();
            try
            {
                var summary = this.GetSummarySetting();
                var status = this.GetValueSetStatusCode();
                var pagerSettings = this.GetPagerSettings();
                var codeSystemGuids = this.GetCodeSystems();

                return summary
                           ? (await this.valueSetSummaryService.GetValueSetSummariesAsync(
                                  pagerSettings,
                                  codeSystemGuids,
                                  status)
                                  .ConfigureAwait(false)).ToValueSetApiModelPage(codeSystemGuids, MapToValueSetItemApiModel)
                           : (await this.valueSetService.GetValueSetsAsync(pagerSettings, codeSystemGuids, status)
                                  .ConfigureAwait(false))
                           .ToValueSetApiModelPage(codeSystemGuids, MapToValueSetApiModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse(
                    ex.Message,
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> SearchAsync()
        {
            this.RequireAccessorAuthorization();
            try
            {
                var model = this.EnsureQueryModel(this.Bind<ValueSetFindByTermQuery>(new BindingConfig { BodyOnly = true }));

                var codeSystemGuids = model.CodeSystemGuids.ToList();

                return model.Summary
                           ? (await this.valueSetSummaryService.GetValueSetSummariesAsync(
                                  model.Term,
                                  model.PagerSettings,
                                  codeSystemGuids,
                                  model.StatusCodes)
                                 .ConfigureAwait(false))
                            .ToValueSetApiModelPage(codeSystemGuids, MapToValueSetItemApiModel)

                           : (await this.valueSetService.GetValueSetsAsync(
                                  model.Term,
                                  model.PagerSettings,
                                  codeSystemGuids,
                                  model.StatusCodes)
                                  .ConfigureAwait(false))
                            .ToValueSetApiModelPage(codeSystemGuids, MapToValueSetApiModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private object AddValueSet()
        {
            this.RequirePublisherAuthorization();
            try
            {
                var model = this.Bind<ClientTermValueSetApiModel>();

                var attempt = this.clientTermCustomizationService.CreateValueSet(model);
                if (!attempt.Success || attempt.Result == null)
                {
                    throw attempt.Exception ?? new ArgumentException("Failed to add value set.");
                }

                var valueSet = attempt.Result;
                var validationResult = this.valueSetValidatorCollection.Validate(valueSet);
                if (validationResult.IsValid)
                {
                    this.clientTermValueSetService.SaveAsNew(valueSet);
                    return this.CreateSuccessfulPostResponse(Mapper.Map<IValueSet, ValueSetApiModel>(valueSet));
                }

                var ex = new InvalidOperationException(
                    "ValueSet validation failed: "
                    + string.Join(Environment.NewLine, validationResult.Errors.Select(vr => vr.ErrorMessage)));
                this.Logger.Error(ex, "Failed to create value set given the model {Model}", model);
                return this.CreateFailureResponse("Failed to create a value set", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse("Failed to create a value set", HttpStatusCode.InternalServerError);
            }
        }

        private object PatchValueSet(string valueSetGuidString)
        {
            this.RequirePublisherAuthorization();
            return this.ParseValueSetGuidAndExecute(valueSetGuidString, PatchValueSetWithValueSetGuid);

            object PatchValueSetWithValueSetGuid(Guid valueSetGuid)
            {
                try
                {
                    var model = this.Bind<ClientTermValueSetApiModel>();

                    var validated = ValueSetPatchHelper.ValidatePatchModel(valueSetGuid, model);
                    if (!validated.Success)
                    {
                        return this.CreateFailureResponse(
                            validated.Exception == null ? "Validation failed" : validated.Exception.Message,
                            HttpStatusCode.BadRequest);
                    }

                    return this.valueSetService.GetValueSet(valueSetGuid)
                        .Select(vs =>
                            {
                                var attempt = this.clientTermCustomizationService.UpdateValueSet(valueSetGuid, model);
                                if (!attempt.Success || attempt.Result == null)
                                {
                                    this.Logger.Error(attempt.Exception, "Failed to path value set {ValueSetGuid}", valueSetGuid);
                                    throw attempt.Exception ?? new InvalidOperationException("Failed to patch value set");
                                }

                                return this.CreateSuccessfulPostResponse(Mapper.Map<IValueSet, ValueSetApiModel>(attempt.Result));
                            })
                        .Else(() =>
                            this.CreateFailureResponse(
                                $"Could not find ValueSet with id: {valueSetGuid}",
                                HttpStatusCode.NotFound));
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "{Message}", ex.Message);
                    return this.CreateFailureResponse("Failed to update a value set", HttpStatusCode.InternalServerError);
                }
            }
        }

        private object DeleteValueSet(string valueSetGuidString)
        {
            this.RequirePublisherAuthorization();
            return this.ParseValueSetGuidAndExecute(valueSetGuidString, DeleteValueSetWithValueSetGuid);

            object DeleteValueSetWithValueSetGuid(Guid valueSetGuid)
            {
                return this.valueSetService.GetValueSet(valueSetGuid)
                    .Select(vs =>
                        {
                            if (vs.StatusCode != ValueSetStatus.Draft)
                            {
                                return this.CreateFailureResponse(
                                    "Invalid ValueSet status.  Delete operation is only permitted for 'Draft' status ValueSets",
                                    HttpStatusCode.BadRequest);
                            }

                            try
                            {
                                this.clientTermValueSetService.Delete(vs);
                                return this.Negotiate.WithStatusCode(HttpStatusCode.OK);
                            }
                            catch (Exception ex)
                            {
                                this.Logger.Error(ex, "{Message}.  ValueSetGuid was: {ValueSetGuid}", ex.Message, valueSetGuid);
                                return this.CreateFailureResponse(
                                    $"Failed to delete ValueSet with id: {valueSetGuid}",
                                    HttpStatusCode.InternalServerError);
                            }
                        })
                    .Else(() =>
                        this.CreateFailureResponse(
                            $"Could not find ValueSet with id: {valueSetGuid}",
                            HttpStatusCode.NotFound));
            }
        }

        private object CopyValueSet()
        {
            this.RequirePublisherAuthorization();
            try
            {
                var model = this.Bind<ValueSetCopyApiModel>();
                EnsureModelHasDefinitionDescription(model);
                var copy = this.valueSetService.GetValueSet(model.OriginGuid)
                            .Select(vs =>
                            {
                                var attempt = this.clientTermValueSetService.Copy(vs, model.Name, model);

                                return attempt.Success
                                           ? this.CreateSuccessfulPostResponse(attempt.Result.ToValueSetApiModel())
                                           : CreateAttemptFailureResponse(attempt);
                            });

                return copy.Else(
                    () => this.CreateFailureResponse(
                        $"ValueSet with matching ID: {model.OriginGuid} was not found",
                        HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse("Failed to copy a value set", HttpStatusCode.InternalServerError);
            }

            //// Published Value Sets may not have definition descriptions which are required when
            //// creating a client term value set.  Last minute change to catch this omission.
            void EnsureModelHasDefinitionDescription(ValueSetCopyApiModel copyModel)
            {
                if (copyModel.DefinitionDescription.IsNullOrWhiteSpace())
                {
                    copyModel.DefinitionDescription = "Unspecified";
                }
            }

            object CreateAttemptFailureResponse(Attempt<IValueSet> attempt)
            {
                if (attempt.Exception != null)
                {
                    this.Logger.Error(attempt.Exception, "Failed to copy value set");
                    return this.CreateFailureResponse(attempt.Exception.Message, HttpStatusCode.InternalServerError);
                }

                return this.CreateFailureResponse("Failed to copy ValueSet", HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> CompareValueSetsAsync()
        {
            this.RequireAccessorAuthorization();
            try
            {
                var model = this.Bind<CompareValueSetsQuery>();

                var comparison = await this.valueSetComparisonService.CompareValueSetCodes(
                                     model.ValueSetGuids,
                                     model.CodeSystemGuids).ConfigureAwait(false);

                return comparison.ToValueSetComparisonResultApiModel(model.CodeSystemGuids.ToArray());
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "{Message}", ex.Message);
                return this.CreateFailureResponse("Failed to compare value sets", HttpStatusCode.InternalServerError);
            }
        }

        private object ChangeStatus(Guid valueSetGuid, string statusCode)
        {
            this.RequirePublisherAuthorization();
            if (Enum.TryParse(statusCode, true, out ValueSetStatus status))
            {
                var attempt = this.clientTermValueSetService.ChangeStatus(valueSetGuid, status);
                try
                {
                    if (attempt.Success)
                    {
                        return this.CreateSuccessfulPostResponse(attempt.Result.ToValueSetApiModel());
                    }

                    if (attempt.Exception != null)
                    {
                        var msg = attempt.Exception.Message;
                        return this.CreateFailureResponse(msg, HttpStatusCode.BadRequest);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "{Message} - ValueSetGuid was {ValueSetGuid}.  Attempted to change to status {Status}", ex.Message, valueSetGuid, status);
                    return this.CreateFailureResponse(
                        $"Failed to change the status code for the value set with id: {valueSetGuid}",
                        HttpStatusCode.InternalServerError);
                }
            }

            return this.CreateFailureResponse($"Failed to cast '{statusCode}' to a valid ValueSetStatus", HttpStatusCode.InternalServerError);
        }

        private bool GetSummarySetting()
        {
            var val = (string)this.Request.Query["$summary"];
            return bool.TryParse(val, out var ret) && ret;
        }

        private IEnumerable<ValueSetStatus> GetValueSetStatusCode()
        {
            var statuses = this.CreateParameterArray((string)this.Request.Query["$status"]);
            foreach (var status in statuses)
            {
                if (!Enum.TryParse(status, true, out ValueSetStatus statusCode))
                {
                    statusCode = ValueSetStatus.Active;
                }

                yield return statusCode;
            }
        }

        private ValueSetFindByTermQuery EnsureQueryModel(ValueSetFindByTermQuery model)
        {
            if (model.PagerSettings == null)
            {
                model.PagerSettings = new PagerSettings
                {
                    CurrentPage = 1,
                    ItemsPerPage = this.Config.TerminologySqlSettings.DefaultItemsPerPage
                };
            }

            if (model.CodeSystemGuids == null)
            {
                model.CodeSystemGuids = Array.Empty<Guid>();
            }

            if (model.Term == null)
            {
                model.Term = string.Empty;
            }

            return model;
        }
    }
}
