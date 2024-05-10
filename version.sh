#!/bin/bash

# Get the latest git tag
 latest_tag=$(git describe --tags --abbrev=0)

# Parse out the version parts
major=$(echo $latest_tag | cut -d. -f1) 
minor=$(echo $latest_tag | cut -d. -f2)
patch=$(echo $latest_tag | cut -d. -f3)

# Increment the patch number
patch=$((patch + 1))

# Reconstruct the version string 
version="$major.$minor.$patch"

# Update AssemblyInfo.cs
perl -pi -e "s/AssemblyInformationalVersion\(\"(.*)\"\)/AssemblyInformationalVersion(\"$version\")/g" AssemblyInfo.cs

# Echo the version 
echo $version