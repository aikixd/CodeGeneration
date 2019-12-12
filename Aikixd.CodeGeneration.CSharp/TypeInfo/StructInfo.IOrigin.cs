using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class StructInfo
    {
        private interface IOrigin
        {
            IEnumerable<MethodMemberInfo> Methods { get; }
            bool IsSealed { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private INamedTypeSymbol symbol;
            private RoslyTypeMembersOrigin membersOrigin;

            public RoslynOrigin(INamedTypeSymbol symbol)
            {
                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
                this.membersOrigin = new RoslyTypeMembersOrigin(symbol);
            }

            public bool IsSealed => this.symbol.IsSealed;

            public IEnumerable<MethodMemberInfo> Methods => this.membersOrigin.Methods;
        }
    }
}
