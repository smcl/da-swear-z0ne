namespace dsz

open System
open System.Collections.Specialized
open System.IO
open System.Net
open System.Text

module Printer =
    let escape (s:string) = s.Replace("\"", "\\\"")
    let defaultSlackMessage = "{{\"username\": \"da swear z0ne\", \"icon_emoji\": \":skull:\", \"text\": \"In *{0}* @ {1}:\n\t{2}\"}}"
    let slackMessageWithChannel = "{{\"username\": \"da swear z0ne\", \"icon_emoji\": \":skull:\", \"text\": \"In *{0}* @ {1}:\n\t{2}\", \"channel\": \"{3}\"}}"

    let createRequestBody (slackChannel:string) (commit:SwearyParser.Commit) = 
        let message = 
            if String.IsNullOrEmpty(slackChannel)
            then String.Format(defaultSlackMessage, commit.Repository.Name, commit.Date, escape (commit.Message.Trim()))
            else String.Format(defaultSlackMessage, commit.Repository.Name, commit.Date, escape (commit.Message.Trim()), slackChannel)

        let body = NameValueCollection()
        body.["payload"] <- message
        body

    let StdOut (commit:SwearyParser.Commit) =
        printfn "%s %s" commit.Hash (commit.Message.Trim())

    let Slack (slackHookUri:string) (slackChannel:string) (commit:SwearyParser.Commit) =
        let wc = new WebClient()
        wc.UploadValues(slackHookUri, "POST", (createRequestBody slackChannel commit)) |> ignore

    let swearyCommitPrinter p (commits:SwearyParser.Commit list) =
        let swearyHashes = commits |> List.map (fun c -> c.Hash) |> Array.ofList
        for commit in commits do p commit
        File.AppendAllLines(".dsz-hashes", swearyHashes)
        0
        
    let Print (print:bool) (slackHookUri:string) (slackChannel:string) = 
        let printMethod = if print then StdOut else Slack slackHookUri slackChannel
        swearyCommitPrinter printMethod
