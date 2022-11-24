# kerbero web api

## Solution setup

Due to the OAuth2 flow for the Nuki API the application must run in particular condition:

- the application must be setup up on an alias of localhost, which adds a second level domain (ex localhost.com or test.com);
- the application must be certified with ssl, I used the following commands (macOS):
  - `dotnet dev-certs https --trust`
  - `x mix gen-dev-crt.sh`
  - `bash src/Kerbero.WebApi/gen-dev-crt.sh`
  - `sudo security add-trusted-cert -d -r trustRoot -k "/Library/Keychains/System.keychain" dev.crt`
  - localhost and its alias must be signed inside the OS and the browser must add an exception on this specific domains.
- after that, you need to apply the migrations to your database. Please refer to the dedicated [section](#how-to-apply-ef-migrations).
- Finally, you should build the solution. kerbero is a .NET6 solution, which can be built with `dotnet build`. \
- You can run or watch the web API launching `dotnet run --project src/Kerbero.WebApi`
  or `dotnet watch --project src/Kerbero.WebApi`.
  
## How to apply EF migrations

The solution is divided in multiple projects and in order to divide responsibility layers, we chose to create migrations 
in a different projects, with the respect to the main project (WebApi). As result, the commands are quiet different
from the base ones.

- **Add Migration** `dotnet ef migrations add <MigrationName> -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.cspro`
- **Remove Migration** `dotnet ef migrations remove`
- **DB update** `dotnet ef database update -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.csproj`. \
