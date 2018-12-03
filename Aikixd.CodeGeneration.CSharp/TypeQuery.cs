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
    /// A query that results in a type.
    /// </summary>
    public sealed class TypeQuery : IFeatureQuery
    {
        private readonly IEnumerable<ITypeFeatureSearchPattern> searchPatterns;
        private readonly IEnumerable<ITypeGeneration> gens;

        public TypeQuery(ITypeFeatureSearchPattern searchPattern, ITypeGeneration typeGeneration)
            : this(new[] { searchPattern }, new[] { typeGeneration })
        { }

        public TypeQuery(IEnumerable<ITypeFeatureSearchPattern> searchPatterns, ITypeGeneration typeGeneration)
            : this(searchPatterns, new[] { typeGeneration })
        { }

        public TypeQuery(ITypeFeatureSearchPattern searchPattern, IEnumerable<ITypeGeneration> typeGenerations)
            : this(new[] { searchPattern }, typeGenerations)
        { }

        public TypeQuery(IEnumerable<ITypeFeatureSearchPattern> searchPatterns, IEnumerable<ITypeGeneration> gens)
        {
            this.searchPatterns = searchPatterns;
            this.gens = gens;
        }

        public IEnumerable<FileGenerationInfo> Execute(SemanticModel semanticModel)
        {
            return
                this
                .searchPatterns
                .SelectMany(x => x.Apply(semanticModel))
                .Distinct()
                .SelectMany(x => 
                    this.gens.Select(y =>
                        y.CreateFileGenerationInfo(x)));
        }
    }
}
