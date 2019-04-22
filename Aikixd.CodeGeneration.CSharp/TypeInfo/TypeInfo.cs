using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public enum TypeKind
    {
        Object,
        Array,
        TypeParameter
    }

    public sealed partial class TypeInfo : IEquatable<TypeInfo>
    {
        private interface IOrigin
        {
            TypeKind Kind { get; }
            string Name { get; }
            string Namespace { get; }
            TypeInfo ContainingType { get; }
            IEnumerable<TypeInfo> TypeParameters { get; }
            Accessibility Accessibility { get; }
            IEnumerable<AttributeInfo> Attributes { get; }

            InterfaceInfo AsInterfaceInfo();
            ClassInfo AsClassInfo();
        }

        private IOrigin origin;

        private TypeInfo(IOrigin origin)
        {
            this.origin = origin;
        }

        public TypeInfo ContainingType => this.origin.ContainingType;

        public IEnumerable<TypeInfo>      TypeParameters => this.origin.TypeParameters;
        public IEnumerable<AttributeInfo> Attributes     => this.origin.Attributes;


        public TypeKind      Kind          => this.origin.Kind;
        public Accessibility Accessibility => this.origin.Accessibility;
        public string        Name          => this.origin.Name;
        public string        Namespace     => this.origin.Namespace;


        public string FullName
        {
            get
            {
                {
                    var r = this.Name;
                    var curContainer = this.ContainingType;

                    while (curContainer != null)
                    {
                        r = $"{curContainer.Name}.{r}";
                        curContainer = curContainer.ContainingType;
                    }

                    if (this.TypeParameters.Any())
                    {
                        r = $"{r}<{ string.Join(", ", this.TypeParameters.Select(x => x.FullName)) }>";
                    }

                    return $"{this.Namespace}.{r}";
                }
            }
        }

        #region Equality
        public bool Equals(TypeInfo other)
        {
            return
                this.Name == other.Name &&
                this.Namespace == other.Namespace &&
                this.ContainingType == other.ContainingType;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeInfo t)
                return this.Equals(t);
            return false;
        }

        public override int GetHashCode()
        {
            int i = 0;

            unchecked
            {
                i += this.Name.GetHashCode() * 15153331;
                i += this.Namespace.GetHashCode() * 742211;
                i += this.ContainingType.GetHashCode() * 43543;
            }

            return i;
        }
        #endregion

        public static TypeInfo Generate(string name, string @namespace, TypeKind kind, Accessibility accessibility)
        {
            return new TypeInfo(
                new NullOrigin(
                    name,
                    @namespace,
                    kind,
                    accessibility));
        }
        
        public static TypeInfo FromSymbol(ITypeSymbol symbol)
        {
            return new TypeInfo(new RoslynOrigin(symbol));
        }

        /// <summary>
        /// Get this type info as interface info. Will not work for generated types.
        /// </summary>
        public InterfaceInfo AsInterface()
        {
            return this.origin.AsInterfaceInfo();
        }

        /// <summary>
        /// Get this type info as class info. Will not work for generated types.
        /// </summary>
        /// <returns></returns>
        public ClassInfo AsClass()
        {
            return this.origin.AsClassInfo();
        }

        private partial class RoslynOrigin : IOrigin
        {
            private ISymbolContainer symbolContainer;

            public RoslynOrigin(ITypeSymbol symbol)
            {
                this.Kind =
                    symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Array         ? TypeKind.Array :
                    symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.TypeParameter ? TypeKind.TypeParameter :
                    TypeKind.Object;

                if (this.Kind == TypeKind.Array)
                    this.symbolContainer = new ArraySymbolContainer((IArrayTypeSymbol)symbol);

                else
                    this.symbolContainer = new RegularSymbolContainer(symbol);
            }

            public TypeKind Kind { get; }

            public string Name      => this.symbolContainer.Name;
            public string Namespace => this.symbolContainer.Namespace;

            public Accessibility              Accessibility  => this.symbolContainer.Accessibility;
            public TypeInfo                   ContainingType => this.symbolContainer.ContainingType;
            public IEnumerable<TypeInfo>      TypeParameters => this.symbolContainer.TypeParameters;
            public IEnumerable<AttributeInfo> Attributes     => this.symbolContainer.Attributes;

            public InterfaceInfo AsInterfaceInfo()
            {
                return this.symbolContainer.AsInterface();
            }

            public ClassInfo AsClassInfo()
            {
                return this.symbolContainer.AsClass();
            }

            private static IEnumerable<TypeInfo> getTypeParameters(ITypeSymbol t)
            {
                if (t is INamedTypeSymbol nt)
                    if (nt.IsGenericType)
                        return
                            nt.TypeArguments
                            .Select(TypeInfo.FromSymbol)
                            .ToArray();

                return Enumerable.Empty<TypeInfo>();
            }

            private static Accessibility getAccessibility(Microsoft.CodeAnalysis.Accessibility access)
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
                        throw new ArgumentOutOfRangeException(nameof(access), $"Class accessibility {access.ToString()} is not supported.");
                }
            }
        }

        private class NullOrigin : IOrigin
        {
            public string Name { get; }
            public string Namespace { get; }
            public TypeKind Kind { get; }
            public Accessibility Accessibility { get; }

            public NullOrigin(string name, string @namespace, TypeKind kind, Accessibility accessibility)
            {
                this.Name          = name;
                this.Namespace     = @namespace;
                this.Kind          = kind;
                this.Accessibility = accessibility;
            }

            public TypeInfo ContainingType => null;

            public IEnumerable<TypeInfo> TypeParameters => Enumerable.Empty<TypeInfo>();

            public IEnumerable<AttributeInfo> Attributes => Enumerable.Empty<AttributeInfo>();

            public ClassInfo AsClassInfo() => null;

            public InterfaceInfo AsInterfaceInfo() => null;
        }
    }
}
