@echo off
@setlocal

cd ext\ffdc-sample-csharp
dotnet run bin\Debug\netcoreapp3.1\ffdc-sample-dotnet3.exe --urls=%1
cd ..\..