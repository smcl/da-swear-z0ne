namespace DaSwearZone

open System
open System.IO
open System.Text.RegularExpressions
open DaSwearZone.BitbucketReader

module SwearFilter = 
    let hashCacheFilename = ".dsz-hashes"

    let containsSpecificSwear commit (swear:string) =
        let r = Regex(String.Format(@"\W{0}\W", swear))
        r.IsMatch(commit.Message)

    let containsAnySwear swearWords commit =
        swearWords
        |> Array.map (containsSpecificSwear commit)
        |> Array.fold (||) false
        
    let getHashes init =
        if init || not (File.Exists(hashCacheFilename)) then 
            set []
        else
            File.ReadAllLines(hashCacheFilename)
            |> Set.ofArray

    let updateHashesFile commits = 
        let hashes = commits |> Array.map (fun c -> c.Hash)
        File.AppendAllLines(hashCacheFilename, hashes)
        commits

    let Apply (init:bool) (commits:Commit []) =
        let swearWords = File.ReadAllLines("swears/en")
        let seenHashes = getHashes init
        in commits
        |> Array.filter (fun c -> not (seenHashes.Contains(c.Hash)))
        |> Array.filter (containsAnySwear swearWords)
        |> updateHashesFile