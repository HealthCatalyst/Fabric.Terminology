namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;

    internal partial class SqlClientTermUnitOfWorkRepository
    {
        // TODO remove after review
        [Obsolete("Does not perform any better than EF")]
        public Attempt<IValueSet> AddBulkInsert(IValueSet valueSet)
        {
            if (!EnsureIsNew(valueSet))
            {
                var invalid = new InvalidOperationException("Cannot save an existing value set as a new value set.");
                return Attempt<IValueSet>.Failed(invalid);
            }

            var valueSetGuid = valueSet.SetIdsForCustomInsert();
            // desc and count sql

            var batchSql = this.PrepareBulkInsertValueSet(valueSet);

            this.uow.Commit(new Queue<BatchSql>(batchSql));

            // Get the updated ValueSet
            var added = this.GetValueSet(valueSetGuid);

            return added.Select(Attempt<IValueSet>.Successful)
                .Else(
                    () => Attempt<IValueSet>.Failed(
                        new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")));
        }

        // TODO remove after review
        private IReadOnlyCollection<BatchSql> PrepareBulkInsertValueSet(IValueSet valueSet)
        {
            var vsd = new ValueSetDescriptionBaseDto(valueSet);
            var counts = valueSet.CodeCounts.Select(count => new ValueSetCodeCountDto(count)).ToList();

            var batchSql = new List<BatchSql> { this.BuildValueSetAndCountsSql(vsd, counts) };

            var codes = valueSet.ValueSetCodes.Select(code => new ValueSetCodeDto(code)).ToList();

            batchSql.AddRange(this.BuildValueSetCodeSql(codes));

            return batchSql;
        }

        // TODO remove after review
        private BatchSql BuildValueSetAndCountsSql(
            ValueSetDescriptionBaseDto vsd,
            IReadOnlyCollection<ValueSetCodeCountDto> counts)
        {
            var parameters = new List<object>();

            var descSql = @"INSERT INTO [ClientTerm].[ValueSetDescriptionBASE]
(BindingID, BindingNM, ValueSetGUID, ValueSetReferenceID, ValueSetNM, VersionDTS, DefinitionDSC, AuthorityDSC, SourceDSC, StatusCD, OriginGUID, ClientCD,
LatestVersionFLG, LastLoadDTS,LastModifiedDTS) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14});";

            parameters.Add(vsd.BindingID);
            parameters.Add(vsd.BindingNM);
            parameters.Add(vsd.ValueSetGUID);
            parameters.Add(vsd.ValueSetReferenceID);
            parameters.Add(vsd.ValueSetNM);
            parameters.Add(vsd.VersionDTS);
            parameters.Add(vsd.DefinitionDSC);
            parameters.Add(vsd.AuthorityDSC);
            parameters.Add(vsd.SourceDSC);
            parameters.Add(vsd.StatusCD);
            parameters.Add(vsd.OriginGUID);
            parameters.Add(vsd.ClientCD);
            parameters.Add(vsd.LatestVersionFLG);
            parameters.Add(vsd.LastLoadDTS);
            parameters.Add(vsd.LastModifiedDTS);

            var countsSql = @"INSERT INTO [ClientTerm].[ValueSetCodeCountBASE] (BindingID, BindingNM, LastLoadDTS, ValueSetGUID, CodeSystemGUID, CodeSystemNM, CodeSystemPerValueSetNBR) VALUES ";
            var paramIndex = 15;
            var valuesSql = string.Empty;
            foreach (var dto in counts)
            {
                if (!valuesSql.IsNullOrWhiteSpace())
                {
                    valuesSql += ",";
                }

                valuesSql += $"({{{paramIndex++}}}, {{{paramIndex++}}}, {{{paramIndex++}}}, {{{paramIndex++}}}, {{{paramIndex++}}}, {{{paramIndex++}}}, {{{paramIndex++}}})";
                parameters.Add(dto.BindingID);
                parameters.Add(dto.BindingNM);
                parameters.Add(dto.LastLoadDTS);
                parameters.Add(dto.ValueSetGUID);
                parameters.Add(dto.CodeSystemGUID);
                parameters.Add(dto.CodeSystemNM);
                parameters.Add(dto.CodeSystemPerValueSetNBR);
            }

            countsSql = $"{countsSql} {valuesSql};";

            return new BatchSql
            {
                Sql = descSql + countsSql,
                Parameters = parameters.ToArray()
            };
        }

        // TODO remove after review
        private IReadOnlyCollection<BatchSql> BuildValueSetCodeSql(IReadOnlyCollection<ValueSetCodeDto> codes)
        {
            var results = new List<BatchSql>();

            // 10 columns - batch 200 should be 2000 params max where limit 2100
            foreach (var batch in codes.Batch(100))
            {
                var paramIndex = 0;
                var parameters = new List<object>();

                var codeBatchSql = @"INSERT INTO [ClientTerm].[ValueSetCodeBASE] 
(BindingID,BindingNM,LastLoadDTS,ValueSetGUID,CodeGUID,CodeCD,CodeDSC,CodeSystemGUID,CodeSystemNM,LastModifiedDTS) VALUES ";

                var codeSql = string.Empty;
                foreach (var dto in batch)
                {
                    if (!codeSql.IsNullOrWhiteSpace())
                    {
                        codeSql += ",";
                    }

                    codeSql +=
                        $"({{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}},{{{paramIndex++}}})";

                    parameters.Add(dto.BindingID);
                    parameters.Add(dto.BindingNM);
                    parameters.Add(dto.LastLoadDTS);
                    parameters.Add(dto.ValueSetGUID);
                    parameters.Add(dto.CodeGUID);
                    parameters.Add(dto.CodeCD);
                    parameters.Add(dto.CodeDSC);
                    parameters.Add(dto.CodeSystemGuid);
                    parameters.Add(dto.CodeSystemNM);
                    parameters.Add(dto.LastModifiedDTS);
                }

                codeBatchSql = $"{codeBatchSql}{codeSql};";

                results.Add(new BatchSql
                {
                    Sql = codeBatchSql,
                    Parameters = parameters.ToArray()
                });
            }

            return results;
        }
    }
}
