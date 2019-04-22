
namespace Aikixd.CodeGeneration.Core

    open System.IO
    open System.Collections.Generic
    open Infrastructure
    open Command

    type OperationResult =
        { Success : bool
          Message : string }

    type ProjectResult =
        { ProjectInfo : ProjectGenerationInfo 
          Success     : bool
          Results     : OperationResult seq }

    type IProjectExplorer =
        abstract member GetGenerationPath : generationRelativePath : string -> string
        abstract member Save : unit -> unit
        abstract member CreateFile    : path : string -> contents : string -> OperationResult
        abstract member OverwriteFile : path : string -> contents : string -> OperationResult
        abstract member RemoveFile    : path : string -> OperationResult

    type ISolutionExplorer =
        abstract member GetProject : projectInfo : ProjectGenerationInfo -> IProjectExplorer
        abstract member Save : unit -> unit

    type Generator 
        ( generationRelativePath : string,
          solExplr : ISolutionExplorer) =

        let getOldFiles affixes path =
            match Directory.Exists(path) with
            | false -> Seq.empty
            | true -> 
                Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                |> Array.toSeq
                |> Seq.filter (fun file -> 
                    Seq.exists (fun affix -> 
                        file.EndsWith(affix)) affixes)

        
        let execute (project : IProjectExplorer) cmd =
            
            let mkName fi =
                [ fi.Name; fi.Group.Affix ]
                |> String.concat "."

            let mkPath fi =
                Path.Combine(
                    project.GetGenerationPath generationRelativePath, 
                    fi.Namespace, 
                    mkName fi)
            
            match cmd with
            | Create fi -> project.CreateFile (mkPath fi) fi.Contents
            | Modify fi -> project.OverwriteFile (mkPath fi) fi.Contents
            | Delete p  -> project.RemoveFile (Path.Combine(project.GetGenerationPath generationRelativePath, p.Value))

        let processProject (projectNfo) =

            let project = solExplr.GetProject projectNfo
            let basePath = project.GetGenerationPath generationRelativePath 

            let getOldFiles' =
                projectNfo.Groups
                |> Seq.map (fun x -> x.Affix)
                |> getOldFiles

            let unroot (path : string) =
                match path.StartsWith(basePath) with
                | true -> Ok (path.Substring(basePath.Length + 1))
                | false -> Error "Wrong path."

            let mapPath (path : string) =
                Ok path
                |> Result.bind unroot
                |> Result.bind (fun x -> LocalPath.create x)

            let (filesForGroups, errors) = 
                basePath
                |> getOldFiles'
                |> Seq.map mapPath
                |> Seq.fold (fun (oks, errs) x -> match x with Ok x -> (x :: oks, errs) | Error x -> (oks, x :: errs)) ([], [])

            match errors with
            | [] -> 
                let exec = execute project

                let results = 
                    iterFileGens (set filesForGroups) (set projectNfo.FileGeneration)
                    |> List.map exec

                project.Save();

                Ok results

            | _ -> Error errors



        let generate(projectNfos) =
            
            let mapErr n es =
                { ProjectInfo = n 
                  Success     = false
                  Results     = es |> Seq.ofList |> Seq.map (fun x -> { Success = false; Message = x }) }

            let mapOk n rs =
                { ProjectInfo = n
                  Success     = true
                  Results     = Seq.ofList rs }

            let fs = 
                projectNfos
                |> Seq.map (fun n -> (n, processProject n))
                |> Seq.map (fun (n, r) -> match r with Ok rs -> mapOk n rs | Error es -> mapErr n es)
                |> List.ofSeq
                        
            solExplr.Save()

            fs
            
        member x.Generate(nfos : HashSet<ProjectGenerationInfo>) =
            generate(nfos) |> Seq.ofList