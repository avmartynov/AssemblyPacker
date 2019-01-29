$VsCommonToolsDir = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\"
$NuGet = "C:\Program Files (x86)\NuGet\nuget.exe"

"Rebuild solution starting... $(Get-Date -Format HH:mm:ss)"
$startDate = $(Get-Date)
try
{
    &"$(Split-Path -Parent $PSCommandPath)\~CleanSolution.ps1"
    
    $ErrorActionPreference = "stop"

    "Start $NuGet locals all -clear"
    &$NuGet locals all -clear
    if ($LASTEXITCODE -ne 0)
    { 
        throw "NuGet package cleaning error. exitCode=$LASTEXITCODE" 
    }

    "Start $NuGet restore $((dir *.sln).Name)"
    &$NuGet restore $((dir *.sln).Name)
    if ($LASTEXITCODE -ne 0)
    { 
        throw "NuGet restore packages error. exitCode=$LASTEXITCODE" 
    }

    "Start MSBuild.exe $((dir *.sln).Name) /t:restore /t:rebuild /p:Configuration=Debug /m:1 /nr:false"
    Invoke-BatchFile "$VsCommonToolsDir\VsDevCmd.bat"    
    MSBuild.exe $((dir *.sln).Name) /t:restore /t:rebuild /m:1 /nr:false
    if ($LASTEXITCODE -ne 0)
    { 
        throw "Build Error. exitCode=$LASTEXITCODE"
    }
    "Rebuild solution SUCCEEDED."
}
catch
{
    $error[0].Exception
    "Rebuild solution FAILED."
    exit 1
}
finally
{
    "Rebuild solution finished at $(Get-Date -Format HH:mm:ss). It takes $(($(Get-Date) - $startDate).TotalSeconds.ToString("N1")) seconds."
}
