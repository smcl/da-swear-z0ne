open System
open System.Collections.Specialized
open System.Net
open System.IO
open dsz 

let urlPattern = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/commits"

[<EntryPoint>]
let main argv =
    let args = dsz.CommandLine.ParseCommandLine (Array.toList argv)
    let swearyCommits = 
        args.repositories
        |> List.collect (fun repo -> dsz.SwearyParser.InspectRepository args.init (String.Format(urlPattern, args.organization, repo)) args.username args.password)

    Printer.Print args.print args.hook swearyCommits