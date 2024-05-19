# Install chocolatey.
if (Get-Command choco -ErrorAction SilentlyContinue) {
  Write-Host "[SurveillanceBot] Chocolatey already installed!" -ForegroundColor Magenta
}
else {
  Write-Host "[SurveillanceBot] Chocolatey is now installing..." -ForegroundColor Red
  Set-ExecutionPolicy Bypass -Scope CurrentUser -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
}

# Install Node.js using Chocolatey.
if (Get-Command npm -ErrorAction SilentlyContinue) {
  Write-Host "[SurveillanceBot] Node.js already installed!" -ForegroundColor Magenta
}
else {
  Write-Host "[SurveillanceBot] Node.js is now installing..." -ForegroundColor Red
  choco install nodejs-lts --version="20.13.1"
}

# Install LocalTunnel using Node.js.
if (Get-Command lt -ErrorAction SilentlyContinue) {
  Write-Host "[SurveillanceBot] LocalTunnel already installed!" -ForegroundColor Magenta
}
else {
  Write-Host "[SurveillanceBot] LocalTunnel is now installing..." -ForegroundColor Red
  npm install -g localtunnel
}

# Build and run the repo.
Write-Host "[SurveillanceBot] Building the software..." -ForegroundColor Magenta
dotnet build $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer -v q

Write-Host "[SurveillanceBot] Running the software..." -ForegroundColor Magenta
dotnet watch run --project $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer