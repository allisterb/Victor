@echo off
@setlocal
set ERROR_CODE=0
dotnet build -c "Debug" src\Interfaces\Victor.CLI\Victor.CLI.csproj %*

:end
exit /B %ERROR_CODE%