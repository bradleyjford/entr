
$ErrorActionPreference = "Stop"

function DownloadWithRetry([string] $url, [string] $downloadLocation, [int] $retries)
{
    while($true)
    {
        try
        {
            Invoke-WebRequest $url -OutFile $downloadLocation
            break
        }
        catch
        {
            $exceptionMessage = $_.Exception.Message

            Write-Host "Failed to download '$url': $exceptionMessage"

            if ($retries -gt 0) 
            {
                $retries--
                Write-Host "Waiting 10 seconds before retrying. Retries left: $retries"
                Start-Sleep -Seconds 10
            }
            else
            {
                $exception = $_.Exception
                throw $exception
            }
        }
    }
}

cd $PSScriptRoot

$repoFolder = $PSScriptRoot
$env:REPO_FOLDER = $repoFolder

$dotnetcoreUrl="https://raw.githubusercontent.com/dotnet/cli/release/2.0.0/scripts/obtain/dotnet-install.ps1"

if ($env:DOTNETCORE_URL)
{
    $dotnetcoreUrl=$env:DOTNETCORE_URL
}

$buildFolder = ".build"
$buildFile="$buildFolder\dotnet-install.ps1"

if (!(Test-Path $buildFolder))
{
    New-Item -ItemType Directory -Force -Path $buildFolder | Out-Null

    Write-Host "Downloading dotnet core from $dotnetcoreUrl"

    DownloadWithRetry -url $dotnetcoreUrl -downloadLocation $buildFile -retries 5

    $cliVersion = "2.0.0-preview1-005977"

    & $buildFolder/dotnet-install.ps1 -Version $cliVersion -InstallDir $buildFolder

    Write-Host "Downloading and installation of the SDK is complete."
}


$dotnet = "$buildFolder/dotnet"

& $dotnet restore Entr.sln
& $dotnet build Entr.sln -v q /nologo