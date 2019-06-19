using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{

    partial class MemberInfo
    {
        public interface IOrigin
        {
            bool   IsStatic { get; }
            string Name     { get; }
            IEnumerable<AttributeInfo> Attributes { get; }
        }
    }

    partial class DataMemberInfo
    {
        new public interface IOrigin : MemberInfo.IOrigin
        {
            TypeInfo Type { get; }
        }
    }

    public sealed partial class FieldMemberInfo
    {
        new public interface IOrigin : DataMemberInfo.IOrigin
        {
            bool IsReadOnly { get; }
        }

        private class RoslynOrigin : IOrigin
        {
            private IFieldSymbol symbol;

            public RoslynOrigin(IFieldSymbol symbol)
            {
                this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            }

            public bool     IsReadOnly => this.symbol.IsReadOnly;
            public bool     IsStatic   => this.symbol.IsStatic;
            public string   Name       => this.symbol.Name;
            public TypeInfo Type       => TypeInfo.FromSymbol(this.symbol.Type);

            public IEnumerable<AttributeInfo> Attributes =>
                this.symbol.GetAttributes().Select(AttributeInfo.Create);
        }
    }

    partial class PropertyMemberInfo
    {
        new public interface IOrigin : DataMemberInfo.IOrigin
        {
            bool IsAutoProperty { get; }
        }

        private class NullOrigin : IOrigin
        {
            public string   Name { get; }
            public TypeInfo Type { get; }

            public bool IsAutoProperty { get; set; } = true;
            public bool IsStatic       { get; set; } = false;

            private IEnumerable<AttributeInfo> attributes = Enumerable.Empty<AttributeInfo>();
            public  IEnumerable<AttributeInfo> Attributes
            {
                get => this.attributes;
                set
                {
                    this.attributes = value ?? throw new ArgumentNullException();
                }
            }

            [Obsolete()]
            public NullOrigin(
                bool isAutoProperty, string name, IEnumerable<AttributeInfo> attributes)
            {
                this.IsAutoProperty = isAutoProperty;
                this.Name           = name;
                this.Attributes     = attributes;
            }

            public NullOrigin(
                string name,
                TypeInfo Type)
            {
                this.Name = name;
                this.Type = Type;
            }
        }

        private class RoslynOrigin : IOrigin
        {
            private IPropertySymbol symbol;

            public RoslynOrigin(IPropertySymbol symbol)
            {
                Debug.Assert(symbol.DeclaringSyntaxReferences.Length == 1);

                this.symbol         = symbol;
                this.IsAutoProperty = isAutoProp();

                bool isAutoProp()
                {
                    var syntax = (PropertyDeclarationSyntax)symbol.DeclaringSyntaxReferences[0].GetSyntax();

                    var getter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.GetAccessorDeclaration));
                    var setter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.SetAccessorDeclaration));
                    var expressionBody = syntax.ExpressionBody?.Kind() == SyntaxKind.ArrowExpressionClause ? syntax.ExpressionBody : null;

                    Debug.Assert(setter != null || getter != null || expressionBody != null, "Property must have a setter or getter.");

                    bool setterAssertedAuto = isAccessorEmpty(setter);
                    bool getterAssertedAuto = isAccessorEmpty(getter) && expressionBody == null;

                    return getterAssertedAuto && setterAssertedAuto;
                }

                bool isAccessorEmpty(AccessorDeclarationSyntax accessor)
                {
                    if (accessor == null)
                        return true;

                    return accessor.Body == null && accessor.ExpressionBody == null;
                }
            }

            public bool IsAutoProperty { get; }

            public string Name => this.symbol.Name;

            public IEnumerable<AttributeInfo> Attributes => this.symbol.GetAttributes().Select(AttributeInfo.Create);

            public TypeInfo Type => TypeInfo.FromSymbol(this.symbol.Type);

            public bool IsStatic => this.symbol.IsStatic;
        }
    }

    partial class MethodMemberInfo
    {
        new private interface IOrigin : MemberInfo.IOrigin
        {
            IEnumerable<ParameterInfo> Parameters { get; }
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

            public bool IsStatic => this.symbol.IsStatic;
        }
    }
}
