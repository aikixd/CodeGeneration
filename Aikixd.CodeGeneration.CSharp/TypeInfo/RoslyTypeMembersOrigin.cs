using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    internal class RoslyTypeMembersOrigin
    {
        private INamedTypeSymbol symbol;

        public RoslyTypeMembersOrigin(INamedTypeSymbol symbol)
        {
            this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        public IEnumerable<MethodMemberInfo> Methods =>
            this.symbol
                .GetMembers()
                .Where(x => x.CanBeReferencedByName)
                .OfType<IMethodSymbol>()
                .Select(MethodMemberInfo.FromSymbol);

        public IEnumerable<PropertyMemberInfo> Properties =>
            this.symbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Select(PropertyMemberInfo.FromSymbol);

        public IEnumerable<FieldMemberInfo> Fields =>
            this.symbol
                .GetMembers()
                .Where(x => x.CanBeReferencedByName)
                .OfType<IFieldSymbol>()
                .Select(FieldMemberInfo.FromSymbol);
    }
}
