

# Build and run the repo.
Write-Host "[SurveillanceBot] Building the software..." -ForegroundColor Magenta
dotnet build $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer -v q

Write-Host "[SurveillanceBot] Running the software..." -ForegroundColor Magenta
dotnet watch run --project $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer