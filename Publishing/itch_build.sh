#!/bin/bash

# Require at least one argument for executable platform
((!$#)) && echo "Please specify the Butler executable platform (e.g. darwin-amd64)" && exit 1

# Set itch push details
itch_title="caps-collective/ftrm"
itch_branch_state="release"
itch_branch_macos="macos"
itch_branch_linux="linux"
itch_branch_windows="windows"

# Find repo root and requested executable
scriptdir=$(dirname "${BASH_SOURCE[0]}")
rootdir=$(cd "$scriptdir/.." ; pwd -P)
butlerexe="$scriptdir/butler/$1/butler"

# Set build details
bindir="$scriptdir/builds"
bin_macos="macos/"
bin_linux="linux/"
bin_windows="windows/Fantasy Town Regional Manager/"

# Discover build version
projsettings="$rootdir/ProjectSettings/ProjectSettings.asset"
appversion=$(grep -o "bundleVersion: [0-9.]*" $projsettings)
appversion="${appversion:15}"
userversion="--userversion $appversion"

# Run butler push for each platform
$butlerexe -V
echo "Starting push from $1 to $itch_title:$itch_branch_state for build v$appversion..."; echo
$butlerexe push --ignore "*.gitkeep" "$bindir/$bin_macos" "$itch_title:$itch_branch_macos-$itch_branch_state" $userversion
$butlerexe push --ignore "*.gitkeep" "$bindir/$bin_linux" "$itch_title:$itch_branch_linux-$itch_branch_state" $userversion
$butlerexe push --ignore "*.gitkeep" "$bindir/$bin_windows" "$itch_title:$itch_branch_windows-$itch_branch_state" $userversion
