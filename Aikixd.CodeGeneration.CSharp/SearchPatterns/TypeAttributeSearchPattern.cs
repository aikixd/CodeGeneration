using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp.SearchPatterns
{
    /// <summary>
    /// Looks for types that are decorated with the provided attribute.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute to search for.</typeparam>
    public sealed class TypeAttributeSearchPattern<TAttr> : ITypeFeatureSearchPattern
    {
        public IEnumerable<INamedTypeSymbol> Apply(Compilation compilation)
        {
            return processNamespace(compilation.GlobalNamespace, new LinkedList<INamedTypeSymbol>());

            //var curNs = compilation.GlobalNamespace;

            //var 

            //    semanticModel.SyntaxTree.GetRoot()
            //    // TODO: Include structs declaration.
            //    .DescendantNodes().OfType<ClassDeclarationSyntax>()
            //    .Where(cls =>
            //    {
            //        var clsModel = semanticModel.GetDeclaredSymbol(cls);
            //        var attrs = clsModel.GetAttributes();

            //        return attrs.Any(x => {
            //            var str = x.AttributeClass.ToDisplayString(
            //                new SymbolDisplayFormat(
            //                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

            //            return str == typeof(TAttr).FullName;
            //        });
            //    })
            //    .Select(x => (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(x))
            //    .ToArray();
        }

        private LinkedList<INamedTypeSymbol> processNamespace(INamespaceSymbol namespaceSymbol, LinkedList<INamedTypeSymbol> list)
        {
            var nsTypes = namespaceSymbol.GetTypeMembers();

            foreach (var t in namespaceSymbol.GetTypeMembers())
                processType(t, list);

            foreach (var ns in namespaceSymbol.GetNamespaceMembers())
                processNamespace(ns, list);

            return list;
        }

        private static void processType(INamedTypeSymbol typeSymbol, LinkedList<INamedTypeSymbol> list)
        {
            if (typeSymbol.GetAttributes().Any(attr =>
                    string.CompareOrdinal(
                        typeof(TAttr).FullName,
                        attr.AttributeClass.ToDisplayString(
                            new SymbolDisplayFormat(
                                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)))
                        == 0))
                list.AddLast(typeSymbol);

            foreach (var t in typeSymbol.GetTypeMembers())
                processType(t, list);
        }
    }
}
