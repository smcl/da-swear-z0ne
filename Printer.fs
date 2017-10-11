namespace dsz

open System
open System.Collections.Specialized
open System.IO
open System.Net
open System.Text
open dsz.SwearyParser
open Newtonsoft.Json

module Printer =
    type SlackMessage = {
        username: string;
        icon_emoji: string;
        text: string;
        channel: string;
        }

    let buildMessage (commit:Commit) = 
        String.Format("In *{0}* @ {1}:\n\t{2}", commit.Repository.Name, commit.Date, commit.Message)

    let createRequestBody (slackChannel:string) (commit:Commit) = 
        let message = {
            username = "da swear z0ne";
            icon_emoji = ":skull:";
            text = buildMessage commit; 
            channel = if String.IsNullOrEmpty(slackChannel) then null else slackChannel
        }
        let body = NameValueCollection()
        body.["payload"] <- JsonConvert.SerializeObject message
        body        

    let StdOut (commit:Commit) =
        printfn "%s" (buildMessage commit)

    let Slack (slackHookUri:string) (slackChannel:string) (commit:Commit) =
        let wc = new WebClient()
        wc.UploadValues(slackHookUri, "POST", (createRequestBody slackChannel commit)) |> ignore

    let swearyCommitPrinter p (commits:Commit list) =
        let swearyHashes = commits |> List.map (fun c -> c.Hash) |> Array.ofList
        for commit in commits do p commit
        File.AppendAllLines(".dsz-hashes", swearyHashes)
        0
        
    let Print (print:bool) (slackHookUri:string) (slackChannel:string) = 
        let printMethod = if print then StdOut else Slack slackHookUri slackChannel
        swearyCommitPrinter printMethod
