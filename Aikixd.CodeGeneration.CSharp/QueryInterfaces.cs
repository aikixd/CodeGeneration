using Aikixd.CodeGeneration.Core;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Aikixd.CodeGeneration.CSharp
{
    /// <summary>
    /// A query pattern that matches types.
    /// </summary>
    public interface ITypeFeatureSearchPattern
    {
        /// <summary>
        /// Executes the search.
        /// </summary>
        /// <param name="semanticModel">The semantic model to look in.</param>
        /// <returns>The type symbol that matches the search.</returns>
        IEnumerable<INamedTypeSymbol> Apply(SemanticModel semanticModel);
    }

    /// <summary>
    /// A query object that finds specific features in the semantic model
    /// and converts them into a <see cref="FileGenerationInfo" />.
    /// </summary>
    public interface IFeatureQuery
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="semanticModel">The semantic model to search in.</param>
        /// <returns>A generation infos for the found feature.</returns>
        IEnumerable<FileGenerationInfo> Execute(SemanticModel semanticModel);
    }

    /// <summary>
    /// Represents a strategy for code generation for type features (classes and structs)
    /// </summary>
    public interface ITypeGeneration
    {
        /// <summary>
        /// Creates the <see cref="FileGenerationInfo"/> for found feature.
        /// </summary>
        /// <param name="symbol">The symbol of the found feature.</param>
        /// <returns>A generation info based on the feature.</returns>
        FileGenerationInfo CreateFileGenerationInfo(INamedTypeSymbol symbol);
    }
}
