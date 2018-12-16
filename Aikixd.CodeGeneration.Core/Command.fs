module Command

open Aikixd.CodeGeneration.Core
open Infrastructure

type GenerationCommand =
    | Create of FileGenerationInfo
    | Modify of FileGenerationInfo
    | Delete of LocalPath.T

let mkCmd fileGenerationInfo oldFileExists =

    match oldFileExists with
    | true -> Modify fileGenerationInfo
    | false -> Create fileGenerationInfo

let iterFileGens oldFiles gens =
    let folder (olds : Set<LocalPath.T>, cmds) (i : FileGenerationInfo) =
        match olds.Contains i.LocalPath with
        | false -> (olds, mkCmd i false :: cmds)
        | true -> 
            (olds.Remove i.LocalPath, mkCmd i true :: cmds)

    let (forDeletion, commands) =
        gens
        |> Set.toSeq
        |> Seq.fold folder (oldFiles, [])

    List.fold (fun cmds i -> Delete i :: cmds) commands (Set.toList forDeletion)
    


        