using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed class InterfaceInfo
    {
        public TypeInfo TypeInfo { get; }
        public string Name => this.TypeInfo.Name;
        public string Namespace => this.TypeInfo.Namespace;
        public string FullName => this.TypeInfo.FullName;

        public Accessibility Accessabilty { get; }

        public IEnumerable<MemberInfo> Members { get; }
        public IEnumerable<PropertyMemberInfo> Properties { get; }
        public IEnumerable<MethodMemberInfo> Methods { get; }

        public IEnumerable<AttributeInfo> Attributes { get; }

        private InterfaceInfo(
            TypeInfo typeInfo,
            Accessibility accessabilty,
            IEnumerable<PropertyMemberInfo> properties,
            IEnumerable<MethodMemberInfo> methods,
            IEnumerable<AttributeInfo> attributes)
        {
            if (typeInfo.Kind != TypeKind.Interface)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(typeInfo),
                    typeInfo,
                    "Provided type info is not an interface.");
            }

            this.TypeInfo     = typeInfo;
            this.Accessabilty = accessabilty;
            this.Properties   = properties;
            this.Methods      = methods;
            this.Attributes   = attributes;

            this.Members =
                properties
                .Cast<MemberInfo>()
                .Union(methods);
        }

        public static InterfaceInfo FromSymbol(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind != Microsoft.CodeAnalysis.TypeKind.Interface)
                return null;

            var props = new LinkedList<PropertyMemberInfo>();
            var methods = new LinkedList<MethodMemberInfo>();

            foreach (var s in symbol.GetMembers())
            {
                switch (s.Kind)
                {
                    case SymbolKind.Property:
                        props.AddLast(PropertyMemberInfo.FromSymbol((IPropertySymbol)s));
                        break;

                    case SymbolKind.Method:
                        if (s.CanBeReferencedByName)
                            methods.AddLast(MethodMemberInfo.FromSymbol((IMethodSymbol)s));
                        break;
                }
            }

            return new InterfaceInfo(
                TypeInfo.FromSymbol(symbol),
                getAccessibility(symbol.DeclaredAccessibility),
                props,
                methods,
                symbol.GetAttributes().Select(AttributeInfo.Create).ToArray());

            Accessibility getAccessibility(Microsoft.CodeAnalysis.Accessibility access)
            {
                switch (access)
                {
                    case Microsoft.CodeAnalysis.Accessibility.NotApplicable:
                        return Accessibility.None;

                    case Microsoft.CodeAnalysis.Accessibility.Private:
                        return Accessibility.Private;

                    case Microsoft.CodeAnalysis.Accessibility.Protected:
                        return Accessibility.Protected;

                    case Microsoft.CodeAnalysis.Accessibility.Public:
                        return Accessibility.Public;

                    case Microsoft.CodeAnalysis.Accessibility.Internal:
                        return Accessibility.Internal;

                    case Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal:
                        return Accessibility.Protected | Accessibility.Internal;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(access), $"Interface accessibility {access.ToString()} is not supported.");
                }
            }
        }
    }
}
