# Name:    StreetRouteCloudAPI
# Purpose: Execute the StreetRouteCloudAPI program

######################### Parameters ##########################
param(
    $startlat = '', 
    $startlong = '', 
    $endlat = '', 
    $endlong = '', 
    $license = '', 
    [switch]$quiet = $false
    )

# Uses the location of the .ps1 file 
# Modify this if you want to use 
$CurrentPath = $PSScriptRoot
Set-Location $CurrentPath
$ProjectPath = "$CurrentPath\StreetRouteDotnet"
$BuildPath = "$ProjectPath\Build"

If (!(Test-Path $BuildPath)) {
  New-Item -Path $ProjectPath -Name 'Build' -ItemType "directory"
}

########################## Main ############################
Write-Host "`n======================== Melissa Street Route Cloud API ========================`n"

# Get license (either from parameters or user input)
if ([string]::IsNullOrEmpty($license) ) {
  $license = Read-Host "Please enter your license string"
}

# Check for License from Environment Variables 
if ([string]::IsNullOrEmpty($license) ) {
  $license = $env:MD_LICENSE 
}

if ([string]::IsNullOrEmpty($license)) {
  Write-Host "`nLicense String is invalid!"
  Exit
}

# Start program
# Build project
Write-Host "`n================================= BUILD PROJECT ================================"

dotnet publish -f="net7.0" -c Release -o $BuildPath StreetRouteDotnet\StreetRouteDotnet.csproj

# Run project
if ([string]::IsNullOrEmpty($startlat) -and [string]::IsNullOrEmpty($startlong) -and [string]::IsNullOrEmpty($endlat) -and [string]::IsNullOrEmpty($endlong)) {

  dotnet $BuildPath\StreetRouteDotnet.dll --license $license 
}
else {
  dotnet $BuildPath\StreetRouteDotnet.dll --license $license --startlat $startlat --startlong $startlong --endlat $endlat --endlong $endlong 
}
