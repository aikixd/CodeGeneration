﻿using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using System;
using System.IO;
using System.Linq;

namespace Aikixd.CodeGeneration.Test.Build
{
    class Program
    {
        class ProjectExplorer : IProjectExplorer
        {
            private readonly Project project;
            private readonly bool defaultCompileItems;

            public ProjectExplorer(Project project)
            {
                this.project = project;
                this.defaultCompileItems = bool.Parse(project.GetProperty("EnableDefaultCompileItems")?.EvaluatedValue ?? bool.FalseString);
            }

            public string GenerationPath =>
                Path.Combine(this.project.DirectoryPath, "AutoGenerated");

            public void CreateFile(string path, string contents)
            {
                if (this.defaultCompileItems == false)
                {
                    var relPath = path.Replace(this.project.DirectoryPath.TrimEnd('\\') + "\\", "");

                    if (project.GetItems("Compile").Any(x => x.EvaluatedInclude == relPath) == false)
                        this.project.AddItem("Compile", relPath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, contents);
            }

            public void Save()
            {
                
            }
        }

        class SolutionExplorer : ISolutionExplorer
        {
            private ProjectCollection projectCollection;

            public SolutionExplorer()
            {
                this.projectCollection = new ProjectCollection();
            }

            public IProjectExplorer GetProject(ProjectGenerationInfo projectInfo)
            {
                return new ProjectExplorer(this.projectCollection.LoadProject(projectInfo.Path));
            }

            public void Save()
            {
                foreach (var p in this.projectCollection.LoadedProjects)
                    p.Save();
            }
        }

        static void Main(string[] args)
        {
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();

            var g = new Generator(new SolutionExplorer());

            g.Generate(new[] {
                new ProjectGenerationInfo(
                    Path.GetFullPath(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            @"..\..\..\..\Aikixd.CodeGeneration.Test.Build.Target.Classic\Aikixd.CodeGeneration.Test.Build.Target.Classic.csproj")),
                    new [] {
                        new FileGenerationInfo(
                            "TestName",
                            "Aikixd.TestNamespace",
                            "Mod",
                            "cs",
                            "Generated at: " + DateTime.Now.ToLongTimeString())
                    }),

                new ProjectGenerationInfo(
                    Path.GetFullPath(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            @"..\..\..\..\Aikixd.CodeGeneration.Test.Build.Target.Netcore\Aikixd.CodeGeneration.Test.Build.Target.Netcore.csproj")),
                    new [] {
                        new FileGenerationInfo(
                            "TestName",
                            "Aikixd.TestNamespace",
                            "Mod",
                            "cs",
                            "Generated at: " + DateTime.Now.ToLongTimeString())
                    })
            });
        }        
    }
}
