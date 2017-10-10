open System
open System.Collections.Specialized
open System.Net
open System.IO
open dsz 

let urlPattern = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/commits"

let swearyCommitPrinter p (commits:SwearyParser.Commit list) =
    let swearyHashes = commits |> List.map (fun c -> c.Hash) |> Array.ofList
    for commit in commits do p commit
    File.AppendAllLines(".dsz-hashes", swearyHashes)
    ()

[<EntryPoint>]
let main argv =
    let args = dsz.CommandLine.ParseCommandLine (Array.toList argv)
    let swearyCommits = 
        args.repositories
        |> List.collect (fun repo -> dsz.SwearyParser.InspectRepository args.init (String.Format(urlPattern, args.organization, repo)) args.username args.password)

    let action = if args.print then Printer.StdOut else Printer.Slack args.hook

    swearyCommitPrinter action swearyCommits
    0 // return an integer exit code
