//using Aikixd.CodeGeneration.Core;
//using Microsoft.Build.Locator;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.MSBuild;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;

//namespace Aikixd.CodeGeneration.CSharp
//{


//    internal class Analyser : IAnalyser
//    {
//        private readonly MSBuildWorkspace workspace;
//        private readonly Solution solution;

//        public Analyser(string solutionPath, string[] projects)
//        {
//            this.workspace = MSBuildWorkspace.Create();

//            this.solution = this.workspace.OpenSolutionAsync(solutionPath).Result;

//            Debugger.Break();
//        }

//        public IEnumerable<ProjectGenerationInfo> GenerateInfo()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
