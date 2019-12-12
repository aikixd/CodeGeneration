using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class DataTypeInfo
    {
        private interface IOrigin
        {
            IEnumerable<PropertyMemberInfo> Properties { get; }
            IEnumerable<FieldMemberInfo> Fields { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private INamedTypeSymbol symbol;
            private RoslyTypeMembersOrigin membersOrigin;

            public RoslynOrigin(INamedTypeSymbol symbol)
            {
                if (symbol.TypeKind != Microsoft.CodeAnalysis.TypeKind.Class &&
                    symbol.TypeKind != Microsoft.CodeAnalysis.TypeKind.Struct)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(symbol),
                        symbol,
                        "Provided symbol is not of a data type kind.");
                }

                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
                this.membersOrigin = new RoslyTypeMembersOrigin(symbol);
            }

            public IEnumerable<PropertyMemberInfo> Properties => this.membersOrigin.Properties;
            public IEnumerable<FieldMemberInfo> Fields => this.membersOrigin.Fields;
        }
    }
}
