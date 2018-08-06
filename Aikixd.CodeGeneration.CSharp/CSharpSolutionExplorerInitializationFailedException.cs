using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aikixd.CodeGeneration.CSharp
{

    public sealed class CSharpSolutionExplorerInitializationFailedException : Exception
    {
        public CSharpSolutionExplorerInitializationFailedException(string message) : base(message) { }
    }
}
