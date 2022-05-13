#!/bin/bash

# Require at least two arguments for username and password
if [ $# -ne 2 ]; then
  echo "Please specify Steam username and password (e.g. steam_build <username> <password>)"
  exit 1
fi

# Get the steampipe builder from the current OS
if [ "$(uname)" == "Darwin" ]; then
  builder="builder_osx"
else
	builder="builder_linux"
fi

# Find repo root and requested executable
scriptdir=$(dirname "${BASH_SOURCE[0]}")
rootdir=$(cd "$scriptdir/../.." ; pwd -P)
steampipeexe="$scriptdir/steampipe/$builder/steamcmd.sh"

# Run Steam app build for each platform
echo "Starting steamcmd..."; echo
$steampipeexe +login $1 $2 +run_app_build ../scripts/app_build_1524530.vdf +quit