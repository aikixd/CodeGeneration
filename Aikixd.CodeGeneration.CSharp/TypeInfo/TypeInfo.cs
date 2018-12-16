using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{
    public enum TypeKind
    {
        Object,
        Array
    }

    public sealed class TypeInfo : IEquatable<TypeInfo>
    {
        private interface IOrigin
        {
            InterfaceInfo AsInterfaceInfo();
            ClassInfo AsClassInfo();
        }

        private class RoslynOrigin : IOrigin
        {
            private ITypeSymbol symbol;

            public RoslynOrigin(ITypeSymbol symbol)
            {
                this.symbol = symbol;
            }

            public InterfaceInfo AsInterfaceInfo()
            {
                if (this.symbol is INamedTypeSymbol nt)
                    return InterfaceInfo.FromSymbol(nt);

                return null;
            }

            public ClassInfo AsClassInfo()
            {
                if (this.symbol is INamedTypeSymbol nt)
                    return ClassInfo.FromSymbol(nt);

                return null;
            }
        }

        private IOrigin origin;

        public string Name { get; }
        public string Namespace { get; }
        public string FullName { get; }

        public TypeInfo ContainingType { get; }
        public IEnumerable<TypeInfo> TypeParameters { get; }
        
        public TypeKind Kind { get; }

        private TypeInfo(
            IOrigin origin,
            string name,
            string @namespace,
            TypeInfo containingType,
            IEnumerable<TypeInfo> typeParameters,
            TypeKind kind)
        {
            this.origin = origin;
            this.Name = name;
            this.Namespace = @namespace;
            this.ContainingType = containingType;
            this.TypeParameters = typeParameters;
            this.Kind = kind;

            this.FullName = getFullName();

            string getFullName()
            {
                var r = name;
                var curContainer = containingType;

                while (curContainer != null)
                {
                    r = $"{curContainer.Name}.{r}";
                    curContainer = curContainer.ContainingType;
                }

                if (this.TypeParameters.Any())
                {
                    r = $"{r}<{ string.Join(", ", this.TypeParameters.Select(x => x.FullName)) }>";
                }

                return $"{@namespace}.{r}";
            }
        }

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

        public static TypeInfo FromSymbol(ITypeSymbol symbol)
        {
            if (symbol.Kind == SymbolKind.ArrayType)
            {
                var arrSymbol = (IArrayTypeSymbol)symbol;
                var elemType = arrSymbol.ElementType;

                return new TypeInfo(
                    new RoslynOrigin(elemType),
                    elemType.Name,
                    elemType.ContainingNamespace.ToDisplayString(),
                    getContainingType(elemType),
                    getTypeParameters(elemType),
                    TypeKind.Array);
            }

            return new TypeInfo(
                new RoslynOrigin(symbol),
                symbol.Name,
                symbol.ContainingNamespace.ToDisplayString(),
                getContainingType(symbol),
                getTypeParameters(symbol),
                TypeKind.Object);

            TypeInfo getContainingType(ITypeSymbol t)
            {
                return 
                    t.ContainingType != null ?
                    TypeInfo.FromSymbol(t.ContainingType) :
                    null;
            }

            IEnumerable<TypeInfo> getTypeParameters(ITypeSymbol t)
            {
                if (t is INamedTypeSymbol nt)
                    if (nt.IsGenericType)
                        return nt.TypeArguments.Select(FromSymbol).ToArray();

                return Enumerable.Empty<TypeInfo>();
            }
        }

        public InterfaceInfo AsInterface()
        {
            return this.origin.AsInterfaceInfo();
        }

        public ClassInfo AsClass()
        {
            return this.origin.AsClassInfo();
        }
    }
}
