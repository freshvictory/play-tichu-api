# play-tichu-api

## Requirements
* Dotnet Core 3.1 SDK (https://dotnet.microsoft.com/download)
* Node.js LTS (12.16.1 works) (https://nodejs.org/en/download/)
* VS Code (or just use dotnet and func command line)
* Azure Functions Core Tools version 3 (npm install -g azure-functions-core-tools@3)
* Nuget.exe (Used by dotnet.  Might come with the SDK, I already had an outdate version on PATH and it got in the way.  I updated to the latest with Chocolatey)

## Run Locally
You'll need a local.settings.json that looks something like this:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureSignalRConnectionString": "XXXXXXXXXXXXXX"
  },  
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "http://localhost:8082,https://play.tichu.cards/",
    "CORSCredentials": true
  }
}
```

Then run these commands (or press F5 in VS Code if you have the Azure Functions extension installed):
```
dotnet build
func host start
```