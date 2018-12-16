using Aikixd.CodeGeneration.Core;
using Microsoft.Build.Evaluation;
using System.IO;
using System.Linq;

namespace Aikixd.CodeGeneration.Test.Build
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

        public string GetGenerationPath(string generationRelativePath)
        {
            return Path.Combine(this.project.DirectoryPath, generationRelativePath);
        }

        public OperationResult CreateFile(string path, string contents)
        {
            if (this.defaultCompileItems == false)
            {
                var relPath = path.Replace(this.project.DirectoryPath.TrimEnd('\\') + "\\", "");

                if (project.GetItems("Compile").Any(x => x.EvaluatedInclude == relPath) == false)
                    this.project.AddItem("Compile", relPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, contents);

            return new OperationResult(true, $"Created: {path}");
        }

        public OperationResult OverwriteFile(string path, string contents)
        {
            File.WriteAllText(path, contents);
            return new OperationResult(true, $"Updated: {path}");
        }

        public OperationResult RemoveFile(string path)
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
            return new OperationResult(true, $"Deleted: {path}");
        }

        public void Save()
        {

        }
    }

}
