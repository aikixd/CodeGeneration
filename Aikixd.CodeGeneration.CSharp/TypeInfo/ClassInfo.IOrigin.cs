using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed partial class ClassInfo
    {
        private interface IOrigin
        {
            IEnumerable<MethodMemberInfo>   Methods    { get; }
            IEnumerable<PropertyMemberInfo> Properties { get; }
            IEnumerable<FieldMemberInfo>    Fields     { get; }
            bool IsStatic { get; }
            bool IsSealed { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private INamedTypeSymbol       symbol;
            private RoslyTypeMembersOrigin membersOrigin;

            public RoslynOrigin(INamedTypeSymbol symbol)
            {
                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
                this.membersOrigin = new RoslyTypeMembersOrigin(symbol);
            }

            public bool IsStatic => this.symbol.IsStatic;
            public bool IsSealed => this.symbol.IsSealed;

            public IEnumerable<MethodMemberInfo>   Methods    => this.membersOrigin.Methods;
            public IEnumerable<PropertyMemberInfo> Properties => this.membersOrigin.Properties;
            public IEnumerable<FieldMemberInfo>    Fields     => this.membersOrigin.Fields;
        }
    }
}
