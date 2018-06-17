
$ErrorActionPreference = "Stop"

& dotnet restore Entr.sln
& dotnet build Entr.sln -v q /nologo -c Release