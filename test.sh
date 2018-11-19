#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"
./build.sh
dotnet test --no-build tests
