namespace Aikixd.CodeGeneration.Core

    open Infrastructure
    open System.Collections.Generic

    type FileGroup =
        { Modifier  : string 
          Extension : string }
        with
        member x.Affix =
            x.Modifier + "." + x.Extension
    
    type FileGenerationInfo =
        { Name      : string
          Namespace : string
          Group     : FileGroup
          Contents  : string }
        with
        member x.LocalPath =
            let name = System.String.Join(".", [| x.Name; x.Group.Modifier; x.Group.Extension |])
            LocalPath.LocalPath (System.IO.Path.Combine(x.Namespace, name))

    type ProjectGenerationInfo =
        { Path           : string
          Groups         : FileGroup seq
          FileGeneration : HashSet<FileGenerationInfo> }