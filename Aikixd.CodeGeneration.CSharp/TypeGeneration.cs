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
        private readonly Func<Compilation, INamedTypeSymbol, string> codeGenFn;

        public TypeGeneration(string fileNameModifier, Func<INamedTypeSymbol, string> codeGenFn)
            : this(fileNameModifier, (c, t) => codeGenFn(t))
        { }
        public TypeGeneration(string fileNameModifier, Func<Compilation, INamedTypeSymbol, string> codeGenFn)
        {
            this.fileNameModifier = fileNameModifier;
            this.codeGenFn = codeGenFn;
        }

        public FileGroup Group => new FileGroup(this.fileNameModifier, "cs");

        public FileGenerationInfo CreateFileGenerationInfo(Compilation compilation, INamedTypeSymbol symbol)
        {
            return new FileGenerationInfo(
                getName(),
                symbol.ContainingNamespace.ToDisplayString(),
                this.Group,
                this.codeGenFn(compilation, symbol));

            string getName()
            {
                var str = symbol.Name;

                var s = symbol;

                while (s.ContainingType != null)
                {
                    s = s.ContainingType;

                    str = $"{s.Name}.{str}";
                }

                return str;
            }
        }
    }
}
