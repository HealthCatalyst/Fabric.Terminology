using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Terminology.Domain
{
    public static partial class Extensions
    {
        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return (value == null) || (value.Trim().Length == 0);
        }
    }
}
