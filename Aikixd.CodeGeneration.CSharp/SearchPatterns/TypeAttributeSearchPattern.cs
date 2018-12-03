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
        public IEnumerable<INamedTypeSymbol> Apply(SemanticModel semanticModel)
        {
            return
                semanticModel.SyntaxTree.GetRoot()
                // TODO: Include structs declaration.
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(cls =>
                {
                    var clsModel = semanticModel.GetDeclaredSymbol(cls);
                    var attrs = clsModel.GetAttributes();

                    return attrs.Any(x => {
                        var str = x.AttributeClass.ToDisplayString(
                            new SymbolDisplayFormat(
                                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

                        return str == typeof(TAttr).FullName;
                    });
                })
                .Select(x => (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(x))
                .ToArray();
        }
    }
}
