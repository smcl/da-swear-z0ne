open System
open System.IO
open dsz 

let urlPattern = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/commits"

let slackSwearyCommits commits = 
    // TODO: dump these into a slack channel, configured via cmd line options
    ()

let printSwearyCommits (commits:SwearyParser.Commit list) = 
    let swearyHashes = commits |> List.map (fun c -> c.Hash) |> Array.ofList
    for commit in commits do printfn "%s: %s" commit.Hash commit.Message
    File.AppendAllLines(".dsz-hashes", swearyHashes)
    ()

[<EntryPoint>]
let main argv =
    let args = dsz.CommandLine.ParseCommandLine (Array.toList argv)
    let swearyCommits = 
        args.repositories
        |> List.collect (fun repo -> dsz.SwearyParser.InspectRepository args.init (String.Format(urlPattern, args.organization, repo)) args.username args.password)

    let action = if args.print then printSwearyCommits else slackSwearyCommits

    action swearyCommits
    0 // return an integer exit code
