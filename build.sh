#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"
which dotnet 2>/dev/null || {
    echo>&2 .NET Core does not appear to be installed on this machine, which is
    echo>&2 required to build the solution. You can install it from the URL below
    echo>&2 and then try building again:
    echo>&2 https://dot.net
    exit 1
}
dotnet --info
dotnet restore
for c in Debug Release; do
    dotnet build --no-restore -c $c
done
