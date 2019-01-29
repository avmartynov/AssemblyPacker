
"Cleaning solution... $(Get-Date -Format HH:mm:ss)"
$startDate = $(Get-Date)
try
{
    $ErrorActionPreference = "continue"
    
    $CleanProject = "$(Split-Path -Parent $PSCommandPath)\~CleanProject.ps1"

    $SolutionName = "$((dir *.sln).Name)"
    Get-Content $SolutionName |
        Select-String 'Project\(' |
            ForEach-Object {
                $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') };
                $projectFilePath = $projectParts[2];
                if ([System.IO.Path]::GetExtension($projectFilePath) -eq ".csproj") {
                    try
                    {
                        Push-Location -Path $(Split-Path -Parent $projectFilePath)
                        &$CleanProject
                    }
                    finally
                    {
                        Pop-Location
                    }
                }
            }

    del -ErrorAction SilentlyContinue *.user
    del -ErrorAction SilentlyContinue -Recurse _ReSharper.Caches
    del -ErrorAction SilentlyContinue -Recurse packages
    del -ErrorAction SilentlyContinue -Recurse -Force .vs
    del -ErrorAction SilentlyContinue -Recurse -Force TestResult.xml
    
    "Cleaning solution SUCCEEDED."
}
catch
{
    $error[0].Exception
    "Cleaning solution FAILED."
    exit 1
}
finally
{
    "Cleaning solution finished at $(Get-Date -Format HH:mm:ss). It takes $(($(Get-Date) - $startDate).TotalSeconds.ToString("N1")) seconds."
}

