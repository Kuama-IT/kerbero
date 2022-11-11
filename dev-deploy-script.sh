# DB set up
echo '\033[4;32mDB setup\033[0m'
cd docker
docker compose up -d
cd ..

# .NET module setup
echo '\033[4;32m.NET module setup\033[0m'
cd web-api
# dotnet ef database update -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.csproj
dotnet build
dotnet run
cd ..

# Vue module setup
echo '\033[4;32mVue module setup\033[0m'
cd web-app
npm run dev
cd ..
