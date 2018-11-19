@echo off
pushd "%~dp0"
call build && dotnet test --no-build tests
popd
