open System
open System.Collections.Specialized
open System.Net
open System.IO
open dsz 

let urlPattern = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/commits"

let escape (s:string) = s.Replace("\"", "\\\"")

let createRequestBody (commit:SwearyParser.Commit) = 
    let messageFormat = "{{\"username\": \"da swear z0ne\", \"text\": \"In {0}: {1}\"}}"
    let body = NameValueCollection()
    body.["payload"] <- String.Format(messageFormat, commit.Repository.Name, escape (commit.Message.Trim()))
    body

let stdoutPrinter (commit:SwearyParser.Commit) =
    printfn "%s %s" commit.Hash (commit.Message.Trim())

let slackPrinter (slackHookUri:string) (commit:SwearyParser.Commit) =
    let wc = new WebClient()
    wc.UploadValues(slackHookUri, "POST", (createRequestBody commit)) |> ignore

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

    let action = if args.print then stdoutPrinter else slackPrinter args.hook

    swearyCommitPrinter action swearyCommits
    0 // return an integer exit code
