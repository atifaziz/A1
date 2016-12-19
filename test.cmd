@echo off
pushd "%~dp0"
call build && dotnet test A1.Tests
popd
