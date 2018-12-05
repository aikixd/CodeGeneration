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

        public AccessabilityInfo Accessabilty { get; }

        public IEnumerable<MemberInfo> Members { get; }
        public IEnumerable<PropertyMemberInfo> Properties { get; }
        public IEnumerable<MethodInfo> Methods { get; }

        public IEnumerable<AttributeInfo> Attributes { get; }

        private InterfaceInfo(
            TypeInfo typeInfo,
            AccessabilityInfo accessabilty,
            IEnumerable<PropertyMemberInfo> properties,
            IEnumerable<MethodInfo> methods,
            IEnumerable<AttributeInfo> attributes)
        {
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
            var props = new LinkedList<PropertyMemberInfo>();
            var methods = new LinkedList<MethodInfo>();

            foreach (var s in symbol.GetMembers())
            {
                switch (s.Kind)
                {
                    case SymbolKind.Property:
                        props.AddLast(PropertyMemberInfo.FromSymbol((IPropertySymbol)s));
                        break;

                    case SymbolKind.Method:
                        if (s.CanBeReferencedByName)
                            methods.AddLast(MethodInfo.FromSymbol((IMethodSymbol)s));
                        break;
                }
            }

            return new InterfaceInfo(
                TypeInfo.FromSymbol(symbol),
                getAccessability(symbol.DeclaredAccessibility),
                props,
                methods,
                symbol.GetAttributes().Select(AttributeInfo.Create).ToArray());

            AccessabilityInfo getAccessability(Accessibility access)
            {
                switch (access)
                {
                    case Accessibility.NotApplicable:
                        return AccessabilityInfo.None;

                    case Accessibility.Private:
                        return AccessabilityInfo.Private;

                    case Accessibility.Protected:
                        return AccessabilityInfo.Protected;

                    case Accessibility.Public:
                        return AccessabilityInfo.Public;

                    case Accessibility.Internal:
                        return AccessabilityInfo.Internal;

                    case Accessibility.ProtectedAndInternal:
                        return AccessabilityInfo.Protected | AccessabilityInfo.Internal;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(access), $"Interface accessability {access.ToString()} is not supported.");
                }
            }
        }
    }
}
