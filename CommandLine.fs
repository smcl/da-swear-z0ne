namespace dsz

module CommandLine =
    type CommandLineOptions = {
        print: bool;
        init: bool;
        username: string;
        password: string;
        organization: string;
        repositories: string list;
        }

    let defaultOptions = {
        print = false;
        init = false;
        username = "";
        password = "";
        organization = "";
        repositories = []
        }

    let rec parseCommandLine' args options = 
        match args with 
        | [] -> 
            options  
        | "--init"::xs ->
            let newOpts = { options with init=true}
            parseCommandLine' xs newOpts 
        | "--print"::xs ->
            let newOpts = { options with print=true}
            parseCommandLine' xs newOpts 
        | "--user"::user::xs -> 
            let newOpts = { options with username=user }
            parseCommandLine' xs newOpts
        | "--pass"::pass::xs -> 
            let newOpts = { options with password=pass }
            parseCommandLine' xs newOpts
        | "--org"::org::xs -> 
            let newOpts = { options with organization=org }
            parseCommandLine' xs newOpts
        | "--repos"::repos::xs -> 
            let newOpts = { options with repositories=repos.Split [|','|] |> List.ofArray }
            parseCommandLine' xs newOpts

    let ParseCommandLine args = 
        parseCommandLine' args defaultOptions