#! /bin/bash

set -e 

dotnet build -c "Debug" src/Interfaces/Victor.CLI/Victor.CLI.csproj $*