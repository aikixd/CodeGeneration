using System;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    [Flags]
    public enum AccessabilityInfo
    {
        None      = 0,
        Inherited = 1 << 0,
        Public    = 1 << 1,
        Private   = 1 << 2,
        Internal  = 1 << 3,
        Protected = 1 << 4
    }
}
