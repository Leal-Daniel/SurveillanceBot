# Install chocolatey.
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install Node.js using Chocolatey.
choco install nodejs-lts --version="20.13.1"

# Install LocalTunnel using Node.js.
npm install -g localtunnel

# Setup process policy to use localtunnel.
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# Build and run the repo.
dotnet build $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer
dotnet watch run