module Infrastructure

[<RequireQualifiedAccess>]
module LocalPath =

    open System.IO

    type T =
        | LocalPath of string
        with
        member x.Value = match x with LocalPath x -> x

    let create path =
        
        let isPath p = 
            try
                Path.GetFullPath(p) |> ignore
                Result.Ok p
            with
                | x -> Error x.Message

        let isRelative p =
            match Path.IsPathRooted(p) with
            | false -> Ok p
            | true -> Error "Path must be relative"

        let r = 
            Ok path
            |> Result.bind isPath
            |> Result.bind isRelative

        match r with
        | Ok p -> Ok <| LocalPath p
        | Error e -> Error e

module Path =

    type path =
        | Path of string
        with
        member x.Value = match x with Path x -> x