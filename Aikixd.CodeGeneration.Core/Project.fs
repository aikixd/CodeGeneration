namespace Aikixd.CodeGeneration.Core

    type FileGenerationInfo =
        { Name      : string
          Namespace : string
          Modifier  : string
          Extension : string
          Contents  : string }

    type ProjectGenerationInfo =
        { Path : string
          FileGeneration : seq<FileGenerationInfo> }