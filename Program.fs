open DaSwearZone

[<EntryPoint>]
let main argv =
    let args = CommandLine.ParseCommandLine (Array.toList argv)
    in args.repositories
    |> Array.collect (fun repo -> BitbucketReader.Get args.organization repo args.username args.password)
    |> SwearFilter.Apply args.init
    |> Printer.Output args.printOnly args.hook args.channel     
    0