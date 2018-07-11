using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aikixd.CodeGeneration.CSharp
{
    public sealed class ClassFeature : IFeature
    {
        public readonly Func<INamedTypeSymbol, string> generateCodeFn;

        public string ClassFullName { get; }
        public string Modifier { get; }

        public ClassFeature(string classFullName, string modifier, Func<INamedTypeSymbol, string> generateCodeFn)
        {
            this.Modifier       = modifier;
            this.generateCodeFn = generateCodeFn ?? throw new ArgumentNullException(nameof(generateCodeFn));
            this.ClassFullName  = classFullName;
        }

        public IEnumerable<IFeatureOccurence> FindOccurences(SemanticModel semanticModel)
        {
            return
                semanticModel.SyntaxTree.GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(cls =>
                {
                    var clsModel = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(cls);

                    return clsModel.ContainingNamespace.ToDisplayString() + "." + clsModel.Name == this.ClassFullName;
                })
                .Select(x => new ClassFeatureOccurence(this, (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(x), x, this.generateCodeFn))
                .ToArray();
        }
    }
}
