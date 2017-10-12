﻿namespace Fabric.Terminology.ElasticSearch.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Nest;

    [ElasticsearchType(Name = "codesetcode")]
    public class CodeSetCodeIndexModel : ICodeSetCode
    {
        [Keyword]
        public Guid CodeGuid { get; set; }

        [Keyword]
        public string Code { get; set; }

        [Text]
        public string Name { get; set; }

        [Keyword]
        public Guid CodeSystemGuid { get; set; }

        [Text]
        public string CodeSystemName { get; set; }
    }
}