// ReSharper disable InconsistentNaming

namespace Fabric.Terminology.SqlServer.Models
{
    using System;

    /// <summary>
    /// Provides HC specific binding information in instances where field information is required
    /// but no binding was used to create the record.
    /// </summary>
    internal class EmptySamdBinding
    {
        public int BindingID => -1000;

        public string BindingNM => "ValueSetCodeClient";


        public DateTime GetLastLoadDTS(DateTime? lastLoad = null)
        {
            return lastLoad ?? DateTime.UtcNow;
        }
    }
}