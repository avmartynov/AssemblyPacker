$Rar = "C:\Program Files (x86)\WinRAR\Rar.exe"

"Rebuild solution starting... $(Get-Date -Format HH:mm:ss)"
$startDate = $(Get-Date)
try
{
    &"$(Split-Path -Parent $PSCommandPath)\~CleanSolution.ps1"
    
    $ErrorActionPreference = "stop"

     # $Rar a -r -m5 -s -rr -t "..\..\..\Arc\$([System.IO.Path]::GetFileNameWithoutExtension((dir *.sln).Name))_$(Get-Date -Format yyyy-MM-dd)" -x.git\* -x.gitignore ..\*
    &$Rar a -r -m5 -s -rr -t -x.git\* -x.gitignore "..\..\..\Arc\$([System.IO.Path]::GetFileNameWithoutExtension((dir *.sln).Name))_$(Get-Date -Format yyyy-MM-dd)" ..\*
    if ($LASTEXITCODE -ne 0)
    { 
        throw "Rar error. exitCode=$LASTEXITCODE" 
    }

    "Archiving  solution SUCCEEDED."
}
catch
{
    $error[0].Exception
    "Archive solution FAILED."
    exit 1
}
finally
{
    "Archive solution finished at $(Get-Date -Format HH:mm:ss). It takes $(($(Get-Date) - $startDate).TotalSeconds.ToString("N1")) seconds."
}
