using Aikixd.CodeGeneration.Core;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp
{

    /// <summary>
    /// Default type generation strategy.
    /// </summary>
    public class TypeGeneration : ITypeGeneration
    {
        private readonly string fileNameModifier;
        private readonly Func<INamedTypeSymbol, string> codeGenFn;

        public TypeGeneration(string fileNameModifier, Func<INamedTypeSymbol, string> codeGenFn)
        {
            this.fileNameModifier = fileNameModifier;
            this.codeGenFn = codeGenFn;
        }

        public FileGenerationInfo CreateFileGenerationInfo(INamedTypeSymbol symbol)
        {
            return new FileGenerationInfo(
                symbol.Name,
                symbol.ContainingNamespace.ToDisplayString(),
                this.fileNameModifier,
                "cs",
                this.codeGenFn(symbol));
        }

    }
}
