helpfunc()
{
   # Display Help
   echo "Setup the project and install dependencies. Be sure to run docker engine."
   echo
   echo "options:"
   echo "-n|--nukisecrets <value>       it helps you setups the Nuki secrets, for developing reason."
   echo "-h|--help                      print this help."
   echo "-e|--emailsecrets <value>      it helps you setups the SendGrid secrets, for developing reason."
   echo
}

# Get secrets if setuped
while [[ $# -gt 0 ]]; do
    case $1 in
        -n|--nukisecrets)
            NUKISECRETS=$2
            shift
            shift
            ;;
        -e|--emailsecrets)
            SENDGRIDSECRETS=$2
            shift
            shift
            ;;
        -h|--help)
            helpfunc
            exit 0
            ;;
        esac
done

if [ ! -z "$NUKISECRETS" ]
then
    echo '\033[4;32mSetting up Nuki secrets...\033[0m'
    cd web-api
    dotnet user-secrets set "NukiExternalOptions:ClientSecret" "$NUKISECRETS" --project src/Kerbero.WebApi/Kerbero.WebApi.csproj
    cd ..
fi

if [ ! -z "$SENDGRIDSECRETS" ]
then
    echo '\033[4;32mSetting up SendGrid secrets...\033[0m'
    cd web-api
    dotnet user-secrets set "EmailSenderServiceOptions:SendGridKey" "$SENDGRIDSECRETS" --project src/Kerbero.WebApi/Kerbero.WebApi.csproj
    cd ..
fi

# DB set up
echo '\033[4;32mDB compose\033[0m'
cd docker
docker compose up -d
cd ..

# .NET module setup
echo '\033[4;32m.NET restore packages\033[0m'
cd web-api
# dotnet ef database update -s src/Kerbero.WebApi/Kerbero.WebApi.csproj -p src/Kerbero.Infrastructure/Kerbero.Infrastructure.csproj
dotnet restore
cd ..

# Vue module setup
echo '\033[4;32mVue module setup\033[0m'
cd web-app
npm install -g husky
npm install
cd ..

