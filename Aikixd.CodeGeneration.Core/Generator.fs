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

    type IGenerationInfoSource =
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

            project.GetGenerationPath generationRelativePath
            |> getOldFiles 
            |> Set.iter (removeFile project)
                        
            projectNfo.FileGeneration
            |> Seq.map (createFile project)


        let generate(projectNfos) =
            
            let mapFn nfo = 
                (nfo.Path, generateForProject nfo);
            
            let fs = 
                projectNfos
                |> Seq.map mapFn
                        
            solExplr.Save()

            fs

        member x.Generate(analyzers : IGenerationInfoSource seq) =
            
            let runOne (a : IGenerationInfoSource) =
                a.GenerateInfo()
                |> generate
                ()
            
            analyzers
            |> Seq.iter runOne
