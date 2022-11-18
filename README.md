# kerbero

## What is kerbero

kerbero is a web application, that allows to interface with multiple smart-lock devices
in a single place.
More information can be found in the GitHub [wiki](https://github.com/Kuama-IT/kerbero/wiki) of the repository.

## Requirements

The projects depend on the following tools and languages:

- dotnet cli
- .NET 6.0+
- C#11
- Vue3.0+
- node and npm

## How to run the project

It is provided a script to setup the project. In order to run the packages download run:
`./kerbero-setup.sh`.
The web-api must be configured in a .env file. There is a file inside the web-api/src/Kerbero.WebApi/ folder named .env-example, which can be renamed and completed.
After the setup, you need to run the script `./dev-deploy-script.sh` to launch the environment. The script run kerbero in a in development environment, as such it needs docker engine to be running.

### Note

The web api needs particular conditions to run. Please, refers to the documentation inside the project to generate the required files and setup the environment properly.
