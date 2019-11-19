@echo off
@setlocal
set ERROR_CODE=0

dotnet build -c "Debug" %*

:end
exit /B %ERROR_CODE%