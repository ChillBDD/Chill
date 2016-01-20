param(
    [int]$buildNumber = 0,
    [string]$task = "default"
)

"Build number $buildNumber"

Import-Module .\build\teamcity.psm1

$nugetexe_path = ".\build\nuget.exe"
if(!(test-path $nugetexe_path))    {
    Invoke-WebRequest "http://nuget.org/nuget.exe" -OutFile $nugetexe_path
}

& $nugetexe_path install ".\Source\.nuget\packages.config" -OutputDirectory ".\Source\packages\"

Import-Module (Get-ChildItem ".\Source\packages\psake.*\tools\psake.psm1" | Select-Object -First 1)
Import-Module .\build\xunit.psm1

Invoke-Psake .\build\default.ps1 $task -framework "4.5.1x64" -properties @{ build_number=$buildNumber }

Remove-Module psake
Remove-Module teamcity
Remove-Module xunit