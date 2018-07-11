namespace Aikixd.CodeGeneration.Core

    open System.IO

    type IProjectExplorer =
        abstract member GetGenerationPath : generationRelativePath : string -> string
        abstract member CreateFile : path : string -> contents : string -> unit
        abstract member RemoveFile : path : string -> unit
        abstract member Save : unit -> unit

    type ISolutionExplorer =
        abstract member GetProject : projectInfo : ProjectGenerationInfo -> IProjectExplorer
        abstract member Save : unit -> unit

    type IFeatureExtractor =
        abstract member FindFeatures : IProjectExplorer -> Feature list

    type IAnalyser =
        abstract member GenerateInfo : unit -> ProjectGenerationInfo seq

    type Generator 
        ( generationRelativePath : string,
          solExplr : ISolutionExplorer) =

        let getOldFiles (genPath) =
            match Directory.Exists(genPath) with
            | false -> Set.empty<string>
            | true ->
                Directory
                    .GetFiles(genPath, "*", SearchOption.AllDirectories)
                    |> Set.ofArray

        let createFile (project : IProjectExplorer) genNfo =
            
            let filename = 
                [ genNfo.Name; genNfo.Modifier; genNfo.Extension ]
                |> List.filter (fun x -> not <| System.String.IsNullOrEmpty(x))
                |> String.concat "."

            let path = Path.Combine(project.GetGenerationPath(generationRelativePath), genNfo.Namespace, filename)

            project.CreateFile path genNfo.Contents

            path

        let removeFile (project : IProjectExplorer) path =
            
            project.RemoveFile path

            ()

        let generateForProject (projectNfo) =
            let project = solExplr.GetProject projectNfo

            let excessFiles = getOldFiles <| project.GetGenerationPath generationRelativePath

            excessFiles
                |> Set.iter (removeFile project)

            let excessFiles = 
                projectNfo.FileGeneration
                |> Seq.map (createFile project)
                |> Seq.fold (fun set cur -> Set.remove cur set) excessFiles

            ()

        member x.Generate(projectNfos) =
            
            projectNfos
            |> Seq.iter generateForProject
                        
            solExplr.Save()

        member x.Generate(analyzers : IAnalyser seq) =
            
            let runOne (a : IAnalyser) =
                a.GenerateInfo()
                |> x.Generate
                ()
            
            analyzers
            |> Seq.iter runOne
