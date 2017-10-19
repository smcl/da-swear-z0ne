namespace DaSwearZone

open System
open System.Net
open System.Text
open Newtonsoft.Json


module BitbucketReader =
    let urlPattern = "https://api.bitbucket.org/2.0/repositories/{0}/{1}/commits"

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

    let createRepoUrl organisation repo = String.Format(urlPattern, organisation, repo)

    let Get (organisation:string) (repo:string) (user:string) (password:string) =
        let wc = new WebClient()
        let credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        wc.Headers.[HttpRequestHeader.Authorization] <- "Basic " + credentials;
        
        let bitbucketResponse = 
            wc.DownloadString(createRepoUrl organisation repo) 
            |> JsonConvert.DeserializeObject<BitbucketCommits>
        in bitbucketResponse.Values