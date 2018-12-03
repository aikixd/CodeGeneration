using Aikixd.CodeGeneration.Core;
using Aikixd.CodeGeneration.CSharp.TypeInfo;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikixd.CodeGeneration.CSharp
{

    public sealed class ProgressReporter : IProgress<ProjectLoadProgress>
    {
        public void Report(ProjectLoadProgress value)
        {
            if (value.Operation == ProjectLoadOperation.Resolve)
                Console.WriteLine($"Project {value.Operation}:\r\n\t{value.FilePath}\r\n\tframework: {value.TargetFramework}");

            else
                Console.WriteLine($"Project {value.Operation}:\r\n\t{value.FilePath}");
        }
    }
}
