#!/bin/bash

set -e 
dotnet build -c "Debug" $*
