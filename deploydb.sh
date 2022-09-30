docker compose up -d
dotnet ef database update -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.csproj
