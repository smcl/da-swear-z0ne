namespace dsz

open System
open System.IO
open System.Net
open System.Text
open Newtonsoft.Json


module SwearyParser =
    let hashCacheFilename = ".dsz-hashes"

    type Repository = {
        Name: string
    }

    type Commit = {
        Hash:string
        Message: string
        Repository: Repository
    }

    type BitbucketCommits =
        { PageLen:int; Values:Commit []}

    // TODO: find a better collection of these
    let swearWords = [
            "shit"; "fuck"; "damn"; "error"
        ] 

    let PopulateHashCache commitHashes =
        let commitHashesArray = commitHashes |> Array.ofList
        File.WriteAllLines(hashCacheFilename, commitHashesArray)
        List.empty

    let GetHashes =
        if not (File.Exists(hashCacheFilename)) then 
            set []
        else
            File.ReadAllLines(hashCacheFilename)
            |> Set.ofArray

    let containsSwear commit =
        let commitMessage = commit.Message.ToLower()
        swearWords 
        |> List.map (fun s -> commitMessage.IndexOf(s) > 0)
        |> List.fold (||) false

    let InspectRepository (init:bool) (repoUrl:string) (user:string) (password:string) =
        let hashes = GetHashes
        let wc = new WebClient()
        let credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        wc.Headers.[HttpRequestHeader.Authorization] <- "Basic " + credentials;
        let commitInfo = wc.DownloadString(repoUrl) |> JsonConvert.DeserializeObject<BitbucketCommits>

        if init then 
            commitInfo.Values 
            |> Array.toList
            |> List.map (fun c -> c.Hash) 
            |> PopulateHashCache
        else 
            commitInfo.Values
            |> Array.toList        
            |> List.filter (fun c -> not (hashes.Contains(c.Hash)))
            |> List.filter (fun c -> containsSwear(c))