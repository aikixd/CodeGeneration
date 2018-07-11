using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp
{
    public sealed class AttributeFeature<TAttribute> : IFeature
        where TAttribute : Attribute
    {
        private readonly Func<INamedTypeSymbol, string> generateCodeFn;

        public string Modifier { get; }

        public AttributeFeature(string modifier, Func<INamedTypeSymbol, string> generateCodeFn)
        {
            this.Modifier = modifier;
            this.generateCodeFn = generateCodeFn ?? throw new ArgumentNullException(nameof(generateCodeFn));
        }

        public IEnumerable<IFeatureOccurence> FindOccurences(SemanticModel semanticModel)
        {
            return
                semanticModel.SyntaxTree.GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(cls =>
                    {
                        var clsModel = semanticModel.GetDeclaredSymbol(cls);
                        var attrs = clsModel.GetAttributes();

                    return attrs.Any(x => {
                        var str = x.AttributeClass.ToDisplayString(
                            new SymbolDisplayFormat(
                                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

                        return str == typeof(TAttribute).FullName;
                    });
                    })
                .Select(x => new ClassFeatureOccurence(this, (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(x), x, this.generateCodeFn))
                .ToArray();
        }
    }
}
