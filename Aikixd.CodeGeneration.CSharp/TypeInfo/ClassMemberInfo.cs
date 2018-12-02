using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public abstract class MemberInfo
    {
        public string Name { get; }

        public IEnumerable<AttributeInfo> Attributes { get; }

        protected MemberInfo(
            string                     name,
            IEnumerable<AttributeInfo> attrs)
        {
            this.Name       = name;
            this.Attributes = attrs;
        }
    }

    public sealed class FieldMemberInfo : MemberInfo
    {
        public TypeInfo Type { get; }
        public bool IsReadOnly { get; }

        public FieldMemberInfo(
            string name,
            TypeInfo type,
            bool isReadOnly,
            IEnumerable<AttributeInfo> attrs)
            : base(name, attrs)
        {
            this.Type = type;
            this.IsReadOnly = isReadOnly;
        }

        public static FieldMemberInfo FromSymbol(IFieldSymbol symbol)
        {
            return new FieldMemberInfo(
                symbol.Name,
                TypeInfo.FromSymbol(symbol.Type),
                symbol.IsReadOnly,
                symbol.GetAttributes().Select(AttributeInfo.Create).ToArray());
        }
    }

    public sealed class PropertyMemberInfo : MemberInfo
    {
        public TypeInfo Type { get; }
        public bool IsAutoProperty { get; }

        public PropertyMemberInfo(
            string                     name,
            TypeInfo                   type,
            bool                       isAutoProperty,
            IEnumerable<AttributeInfo> attrs)
            : base(name, attrs)
        {
            this.Type = type;
            this.IsAutoProperty = isAutoProperty;
        }

        internal static PropertyMemberInfo FromSymbol(IPropertySymbol s)
        {
            Debug.Assert(s.DeclaringSyntaxReferences.Length == 1);

            bool isAutoProp;

            var syntax = (PropertyDeclarationSyntax)s.DeclaringSyntaxReferences[0].GetSyntax();

            var getter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.GetAccessorDeclaration));
            var setter = syntax.AccessorList?.Accessors.FirstOrDefault(x => x.IsKind(SyntaxKind.SetAccessorDeclaration));

            Debug.Assert(setter != null || getter != null, "Property must have a setter or getter.");

            bool setterAssertedAuto = setter != null ? setter.Body == null : true;
            bool getterAssertedAuto = getter != null ? getter.Body == null : true;

            isAutoProp = getterAssertedAuto && setterAssertedAuto;
            

            return new PropertyMemberInfo(
                s.Name,
                TypeInfo.FromSymbol(s.Type),
                isAutoProp,
                s.GetAttributes().Select(AttributeInfo.Create));
        }
    }

    public sealed class MethodInfo : MemberInfo
    {
        public TypeInfo ReturnType { get; }
        public IEnumerable<ParameterInfo> Parameters { get; }

        public MethodInfo(
            string name,
            TypeInfo returnType,
            IEnumerable<ParameterInfo>  arguments,
            IEnumerable<AttributeInfo> attrs)
            : base(name, attrs)
        {
            this.ReturnType = returnType;
            this.Parameters  = arguments;
        }

        internal static MethodInfo FromSymbol(IMethodSymbol symbol)
        {
            try
            {
                return new MethodInfo(
                    symbol.Name,
                    TypeInfo.FromSymbol(symbol.ReturnType),
                    symbol.Parameters.Select(ParameterInfo.FromSymbol).ToArray(),
                    symbol.GetAttributes().Select(AttributeInfo.Create).ToArray());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not create method info for {{{symbol}}}. See inner exception for details.", e);
            }
        }
    }
}
