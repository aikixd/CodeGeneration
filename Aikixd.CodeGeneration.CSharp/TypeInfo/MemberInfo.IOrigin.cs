using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{

    public sealed partial class FieldMemberInfo
    {
        private interface IOrigin
        {
            bool                       IsReadOnly { get; }
            string                     Name       { get; }
            IEnumerable<AttributeInfo> Attributes { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private IFieldSymbol symbol;

            public RoslynOrigin(IFieldSymbol symbol)
            {
                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            }

            public bool   IsReadOnly => this.symbol.IsReadOnly;
            public string Name       => this.symbol.Name;

            public IEnumerable<AttributeInfo> Attributes =>
                this.symbol.GetAttributes().Select(AttributeInfo.Create);
        }
    }

    partial class PropertyMemberInfo
    {
        private interface IOrigin
        {
            bool IsAutoProperty { get; }
            string Name { get; }
            IEnumerable<AttributeInfo> Attributes { get; }
        }

        private class NullOrigin : IOrigin
        {
            public bool IsAutoProperty { get; }

            public string Name { get; }

            public IEnumerable<AttributeInfo> Attributes { get; }

            public NullOrigin(bool isAutoProperty, string name, IEnumerable<AttributeInfo> attributes)
            {
                this.IsAutoProperty = isAutoProperty;
                this.Name = name;
                this.Attributes = attributes;
            }
        }

        private class RoslynOrigin : IOrigin
        {
            private IPropertySymbol symbol;

            public RoslynOrigin(IPropertySymbol symbol)
            {
                this.symbol = symbol;

                Debug.Assert(symbol.DeclaringSyntaxReferences.Length == 1);

                bool isAutoProp;

                var syntax = (PropertyDeclarationSyntax)symbol.DeclaringSyntaxReferences[0].GetSyntax();

                var getter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.GetAccessorDeclaration));
                var setter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.SetAccessorDeclaration));
                var expressionBody = syntax.ExpressionBody?.Kind() == SyntaxKind.ArrowExpressionClause ? syntax.ExpressionBody : null;

                Debug.Assert(setter != null || getter != null || expressionBody != null, "Property must have a setter or getter.");

                bool setterAssertedAuto = setter != null ? setter.Body == null : true;
                bool getterAssertedAuto = getter != null ? getter.Body == null : expressionBody == null;

                this.IsAutoProperty = getterAssertedAuto && setterAssertedAuto;
            }

            public bool IsAutoProperty { get; }

            public string Name => this.symbol.Name;

            public IEnumerable<AttributeInfo> Attributes => this.symbol.GetAttributes().Select(AttributeInfo.Create);
        }
    }

    partial class MethodMemberInfo
    {
        private interface IOrigin
        {
            IEnumerable<ParameterInfo> Parameters { get; }
            string                     Name       { get; }
            IEnumerable<AttributeInfo> Attributes { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private IMethodSymbol symbol;

            public RoslynOrigin(IMethodSymbol symbol)
            {
                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            }

            public IEnumerable<ParameterInfo> Parameters => 
                this.symbol.Parameters.Select(ParameterInfo.FromSymbol).ToArray();

            public string Name => this.symbol.Name;

            public IEnumerable<AttributeInfo> Attributes => 
                this.symbol.GetAttributes().Select(AttributeInfo.Create);
        }
    }
}
