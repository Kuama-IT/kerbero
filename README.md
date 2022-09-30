# kerbero
## What is kerbero
kerbero is a web application, that allows to interface with multiple smart-lock devices
in a single place.
More information can be found in the GitHub [wiki](https://github.com/Kuama-IT/kerbero/wiki) of the repository.

## How to run the project
Due to the OAuth2 flow for the Nuki API the application must run in particular condition:
- the application must be setup up on an alias of localhost, which adds a second level domain (ex localhost.com or test.com);
- the application must be certified with ssl, I used the following commands (macOS):
  -  `dotnet dev-certs https --trust`
  - `x mix gen-dev-crt.sh`
  - `bash src/Kerbero.WebApi/gen-dev-crt.sh`
  - `sudo security add-trusted-cert -d -r trustRoot -k "/Library/Keychains/System.keychain" dev.crt`
- after that, localhost and its alias must be signed inside the OS and the browser must add an exception on this specific domains.
Then, it must be applied the migrations to your database, with `dotnet ef database update -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.csproj`. \
kerbero is a .NET6 solution, which can be built with `dotnet build`. \
You can run or watch the web API launching `dotnet run --project src/Kerbero.WebApi` 
or `dotnet watch --project src/Kerbero.WebApi`.