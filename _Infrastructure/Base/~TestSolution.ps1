$SolutionName = [System.IO.Path]::GetFileNameWithoutExtension("$((dir *.sln).Name)")

"Tests starting... $(Get-Date -Format HH:mm:ss)"
$startDate = $(Get-Date)
try
{
    &".\AllTests\bin\Debug\Twidlle.$SolutionName.AllTests.exe" --where "test=Twidlle.Infrastructure.Testing.TestSetFixture"
    if ($LASTEXITCODE -ne 0)
    { 
        throw "Test Error. exitCode=$LASTEXITCODE"
    }
    "Tests SUCCEEDED."
}
catch
{
    $error[0].Exception
    "Test FAILED."
    exit 1
}
finally
{
    "Test finished at $(Get-Date -Format HH:mm:ss). It takes $(($(Get-Date) - $startDate).TotalSeconds.ToString("N1")) seconds."
}
