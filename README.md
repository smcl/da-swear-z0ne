# da-swear-z0ne
Similar to @gitlost - poll bitbucket repositories in an organization, find any new commits that include swear words and share in slack

## Requirements:

* .NET Core 2.0
* F# v...something
* A login to Bitbucket
* A slack "Incoming Webhook" URL (see: https://api.slack.com/custom-integrations/incoming-webhooks)

## Usage

Assuming the following:
* username: `testperson`
* password: `password`
* bitbucket organisation (to which the repositories belong): `borkltd"
* repositories to search: `foo` and `bar`
* slack webhook: `https://hooks.slack.com/services/blah/blorp/blhhh`

$ dotnet restore
$ dotnet build
$ dotnet run --user testperson --pass password --org bork --repos foo,bar --hook https://hooks.slack.com/services/blah/blorp/blhhh

This will take the latest 30 commits in each, scan them for some swear words, log them in the slack channel and cache their hashes in a file `.dsz-cache` in the current working directory (which we'll check in future so we don't send any commit more than once).

To print to stdout instead of sending to slack, use the `--print` option