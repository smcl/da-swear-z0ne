open System
open System.Collections.Specialized
open System.Net
open System.IO
open dsz

[<EntryPoint>]
let main argv =
    let args = dsz.CommandLine.ParseCommandLine (Array.toList argv)
    in args.repositories
    |> List.collect (fun repo -> BitbucketReader.Get args.organization repo args.username args.password)
    |> SwearFilter.Apply args.init
    |> Printer.Output args.printOnly args.hook args.channel     
    0