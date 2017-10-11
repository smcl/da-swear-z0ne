namespace dsz

open System
open System.IO
open System.Net
open System.Text
open System.Text.RegularExpressions
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
        Date: DateTime
    }

    type BitbucketCommits =
        { PageLen:int; Values:Commit []}

    let swearWords = 
        File.ReadAllLines("swears/en")
        |> List.ofArray

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

    let containsSpecificSwear commit (swear:string) =
        let r = Regex(String.Format(@"\W{0}\W", swear))
        r.IsMatch(commit.Message)

    let containsAnySwear commit =
        let commitMessage = commit.Message.ToLower()
        swearWords 
        |> List.map (containsSpecificSwear commit)
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
            |> List.filter (fun c -> containsAnySwear(c))