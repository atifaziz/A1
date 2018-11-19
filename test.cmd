@echo off
setlocal
pushd "%~dp0"
call build && dotnet test --no-build tests
set TEST=dotnet test --no-build tests -c
    call build ^
 && %TEST% Debug -p:CollectCoverage=true ^
                 -p:CoverletOutputFormat=opencover ^
                 -p:Exclude=[XUnit*]* ^
 && %TEST% Release
popd
