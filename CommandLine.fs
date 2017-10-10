namespace dsz

module CommandLine =
    type CommandLineOptions = {
        print: bool;
        init: bool;
        username: string;
        password: string;
        organization: string;
        repositories: string list;
        hook: string;
        }

    let defaultOptions = {
        print = false;
        init = false;
        username = "";
        password = "";
        organization = "";
        repositories = [];
        hook = "";
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
        | "--hook"::hook::xs -> 
            let newOpts = { options with hook=hook }
            parseCommandLine' xs newOpts

    let ParseCommandLine args = 
        parseCommandLine' args defaultOptions