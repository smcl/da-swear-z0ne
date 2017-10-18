namespace dsz

open System
open System.IO
open System.Text.RegularExpressions
open dsz.BitbucketReader

module SwearFilter = 
    let hashCacheFilename = ".dsz-hashes"

    let containsSpecificSwear commit (swear:string) =
        let r = Regex(String.Format(@"\W{0}\W", swear))
        r.IsMatch(commit.Message)

    let containsAnySwear commit =
        let commitMessage = commit.Message.ToLower()
        File.ReadAllLines("swears/en")
        |> List.ofArray 
        |> List.map (containsSpecificSwear commit)
        |> List.fold (||) false
        
    let getHashes init =
        if init || not (File.Exists(hashCacheFilename)) then 
            set []
        else
            File.ReadAllLines(hashCacheFilename)
            |> Set.ofArray

    let updateHashesFile (commits:Commit list) = 
        let hashes = commits |> List.map (fun c -> c.Hash) |> Array.ofList
        File.AppendAllLines(hashCacheFilename, hashes)
        commits

    let Apply (init:bool) (commits:Commit list) = 
        let seenHashes = getHashes init
        in commits
            |> List.filter (fun c -> not (seenHashes.Contains(c.Hash)))
            |> List.filter containsAnySwear
            |> updateHashesFile