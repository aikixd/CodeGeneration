using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.TypeInfo
{

    public sealed partial class TypeInfo
    {
        private partial class RoslynOrigin
        {
            private interface ISymbolContainer
            {
                string Name { get; }
                string Namespace { get; }
                TypeInfo ContainingType { get; }
                IEnumerable<TypeInfo> TypeParameters { get; }
                Accessibility Accessibility { get; }
                IEnumerable<AttributeInfo> Attributes { get; }

                ClassInfo AsClass();
                InterfaceInfo AsInterface();
                StructInfo AsStruct();
            }

            private class RegularSymbolContainer : ISymbolContainer
            {
                private ITypeSymbol symbol;

                public string Name => this.symbol.Name;
                public string Namespace => this.symbol.ContainingNamespace.ToDisplayString();

                public TypeInfo ContainingType => 
                    this.symbol.ContainingType != null 
                    ? TypeInfo.FromSymbol(this.symbol.ContainingType) 
                    : null;

                public IEnumerable<TypeInfo> TypeParameters => getTypeParameters(this.symbol);

                public Accessibility Accessibility => getAccessibility(this.symbol.DeclaredAccessibility);

                public IEnumerable<AttributeInfo> Attributes =>
                    this.symbol.GetAttributes().Select(AttributeInfo.Create).ToArray();

                public RegularSymbolContainer(ITypeSymbol symbol)
                {
                    this.symbol = symbol;
                }

                public ClassInfo AsClass()
                {
                    if (this.symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Class)
                        return ClassInfo.FromSymbol((INamedTypeSymbol)symbol);

                    return null;
                }

                public InterfaceInfo AsInterface()
                {
                    if (this.symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Interface)
                        return InterfaceInfo.FromSymbol((INamedTypeSymbol)symbol);

                    return null;
                }

                public StructInfo AsStruct()
                {
                    if (this.symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct)
                        return StructInfo.FromSymbol((INamedTypeSymbol)symbol);

                    return null;
                }
            }

            private class ArraySymbolContainer : ISymbolContainer
            {
                IArrayTypeSymbol arrSymbol;
                ITypeSymbol elemSymbol;

                public string Name => this.elemSymbol.Name;
                public string Namespace => this.elemSymbol.ContainingNamespace.ToDisplayString();

                public TypeInfo ContainingType =>
                    this.elemSymbol.ContainingType != null
                    ? TypeInfo.FromSymbol(this.elemSymbol.ContainingType)
                    : null;

                public IEnumerable<TypeInfo> TypeParameters => getTypeParameters(this.elemSymbol);

                public Accessibility Accessibility => getAccessibility(this.elemSymbol.DeclaredAccessibility);

                public IEnumerable<AttributeInfo> Attributes => 
                    this.elemSymbol.GetAttributes().Select(AttributeInfo.Create).ToArray();

                public ArraySymbolContainer(IArrayTypeSymbol symbol)
                {
                    this.arrSymbol = symbol;
                    this.elemSymbol = symbol.ElementType;
                }

                public ClassInfo AsClass()
                {
                    if (this.elemSymbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Class)
                        return ClassInfo.FromSymbol((INamedTypeSymbol)elemSymbol);

                    return null;
                }

                public InterfaceInfo AsInterface()
                {
                    if (this.elemSymbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Interface)
                        return InterfaceInfo.FromSymbol((INamedTypeSymbol)elemSymbol);

                    return null;
                }

                public StructInfo AsStruct()
                {
                    if (this.elemSymbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct)
                        return StructInfo.FromSymbol((INamedTypeSymbol)elemSymbol);

                    return null;
                }
            }
        }
    }
}
