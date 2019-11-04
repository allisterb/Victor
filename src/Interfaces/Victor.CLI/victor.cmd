@echo off
@setlocal
set ERROR_CODE=0

REM dotnet ".\bin\Debug\netcoreapp2.1\Victor.CLI.dll" %*
".\bin\Debug\net461\Victor.CLI.exe" %*
goto end

:end
exit /B %ERROR_CODE%