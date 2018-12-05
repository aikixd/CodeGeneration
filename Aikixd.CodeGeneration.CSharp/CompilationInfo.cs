using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp
{
    public class CompilationInfo
    {
        private readonly Compilation compilation;

        private CompilationInfo(Compilation compilation)
        {
            this.compilation = compilation;
        }

        public InterfaceInfo FindInterface(TypeInfo.TypeInfo typeInfo)
        {
            var r = this.compilation.GetSymbolsWithName(
                x => x.Contains(typeInfo.Name),
                SymbolFilter.Type);


            throw new NotImplementedException();
        }

        public static CompilationInfo FromCompilation(Compilation compilation)
        {
            return new CompilationInfo(compilation);
        }
    }
}
