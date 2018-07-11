using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public sealed class ClassInfo
    {
        public string Name { get; }
        public string Namespace { get; }

        public AccessabilityInfo Accessabilty { get; }

        public bool IsStatic { get; }
        public bool IsSealed { get; }

        public IEnumerable<MemberInfo>         Members { get; }
        public IEnumerable<FieldMemberInfo>    Fields { get; }
        public IEnumerable<PropertyMemberInfo> Properties { get; }
        public IEnumerable<MethodInfo>   Methods { get; }

        public IEnumerable<AttributeInfo> Attributes { get; }

        public ClassInfo(
            string name,
            string @namespace,
            bool isStatic,
            bool isSealed,
            AccessabilityInfo accessabilty,
            IEnumerable<FieldMemberInfo>    fields,
            IEnumerable<PropertyMemberInfo> properties,
            IEnumerable<MethodInfo>         methods,
            IEnumerable<AttributeInfo>      attributes)
        {
            this.Name      = name;
            this.Namespace = @namespace;

            this.IsStatic = isStatic;
            this.IsSealed = isSealed;

            this.Accessabilty = accessabilty;

            this.Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));

            this.Fields     = fields     ?? throw new ArgumentNullException(nameof(fields));
            this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            this.Methods    = methods    ?? throw new ArgumentNullException(nameof(methods));

            this.Members =
                this.Properties
                .Cast<MemberInfo>()
                .Union(this.Methods)
                .Union(this.Fields);
        }

        public static ClassInfo FromSymbol(INamedTypeSymbol symbol)
        {
            var fields = new LinkedList<FieldMemberInfo>();
            var props = new LinkedList<PropertyMemberInfo>();
            var methods = new LinkedList<MethodInfo>();

            foreach (var s in symbol.GetMembers())
            {
                switch (s.Kind)
                {
                    case SymbolKind.Field:
                        if (s.CanBeReferencedByName)
                            fields.AddLast(FieldMemberInfo.FromSymbol((IFieldSymbol)s));
                        break;
                        
                    case SymbolKind.Property:
                        props.AddLast(PropertyMemberInfo.FromSymbol((IPropertySymbol)s));
                        break;

                    case SymbolKind.Method:
                        if (s.CanBeReferencedByName)
                            methods.AddLast(MethodInfo.FromSymbol((IMethodSymbol)s));
                        break;
                }
            }

            return new ClassInfo(
                symbol.Name,
                symbol.ContainingNamespace.ToDisplayString(),
                symbol.IsStatic,
                symbol.IsSealed,
                getAccessability(symbol.DeclaredAccessibility),
                fields,
                props,
                methods,
                symbol.GetAttributes().Select(AttributeInfo.Create));

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
                        throw new ArgumentOutOfRangeException(nameof(access), $"Class accessability {access.ToString()} is not supported.");
                }
            }
        }
    }
}
