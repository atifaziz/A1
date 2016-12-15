@echo off
pushd "%~dp0"
call :main
popd
goto :EOF

:main
setlocal
set DOTNETEXE=
for %%f in (dotnet.exe) do set DOTNETEXE=%%~dpnx$PATH:f
if not defined DOTNETEXE set DOTNETEXE=%ProgramFiles%\dotnet
if not exist "%DOTNETEXE%x" (
    echo>&2 .NET Core does not appear to be installed on this machine, which is
    echo>&2 required to build the solution. You can install it from the URL below
    echo>&2 and then try building again:
    echo>&2 https://dot.net
    exit /b 1
)
"%DOTNETEXE%" --info ^
  && "%DOTNETEXE%" restore ^
  && call :build Debug ^
  && call :build Release
goto :EOF

:build
"%DOTNETEXE%" build -c %1 A1
goto :EOF
