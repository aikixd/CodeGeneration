using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using System.IO;
using System.Linq;

namespace Aikixd.CodeGeneration.CSharp
{
    public sealed class CSharpProjectExplorer : IProjectExplorer
    {
        private readonly Project project;
        private readonly bool defaultCompileItems;

        public CSharpProjectExplorer(Project project)
        {
            this.project = project;
            this.defaultCompileItems = bool.Parse(project.GetProperty("EnableDefaultCompileItems")?.EvaluatedValue ?? bool.FalseString);
        }
        
        public string GetGenerationPath(string generationRelativePath)
        {
            return Path.Combine(this.project.DirectoryPath, generationRelativePath);
        }

        public void CreateFile(string path, string contents)
        {
            if (this.defaultCompileItems == false)
            {
                var relPath = path.Replace(this.project.DirectoryPath.TrimEnd('\\') + "\\", "");

                if (this.project.GetItems("Compile").Any(x => x.EvaluatedInclude == relPath) == false)
                    this.project.AddItem("Compile", relPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, contents);
        }


        public void RemoveFile(string path)
        {
            if (this.defaultCompileItems == false)
            {
                var relPath = path.Replace(this.project.DirectoryPath.TrimEnd('\\') + "\\", "");

                this.project.RemoveItems(
                    this.project
                    .GetItems("Compile")
                    .Where(x => x.EvaluatedInclude == relPath));
                    
            }

            File.Delete(path);
        }

        public void Save()
        {
            // No need to save the project file, it will be saved at the end.
        }
    }
}
