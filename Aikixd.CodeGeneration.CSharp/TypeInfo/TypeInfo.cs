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
        public string Name { get; }
        public string Namespace { get; }
        public TypeKind Kind { get; }

        public TypeInfo(
            string name,
            string @namespace,
            TypeKind kind)
        {
            this.Name = name;
            this.Namespace = @namespace;
            this.Kind = kind;
        }

        public bool Equals(TypeInfo other)
        {
            return
                this.Name == other.Name &&
                this.Namespace == other.Namespace;
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
            }

            return i;
        }

        public static TypeInfo FromSymbol(ITypeSymbol symbol)
        {
            if (symbol.Kind == SymbolKind.ArrayType)
            {
                var arrSymbol = (IArrayTypeSymbol)symbol;
                var elemType = arrSymbol.ElementType;

                return new TypeInfo(elemType.Name, elemType.ContainingNamespace.ToDisplayString(), TypeKind.Array);
            }

            return new TypeInfo(symbol.Name, symbol.ContainingNamespace.ToDisplayString(), TypeKind.Object);
        }
    }
}
