#!/bin/bash

#########################################################
# Replace the Package version in the snapcraft.yaml file 
# with the value in the project file
version=$(sed -n 's/.*<Version>\(.*\)<\/Version>.*/\1/p' slack-send.csproj)
echo "Version in Project: '$version', Updating snapcraft.yaml"
snapcraft_content=$(cat snap/snapcraft.yaml)
# shellcheck disable=SC2001
updated_content=$(echo "$snapcraft_content" | sed "s/version: .*/version: $version/")
echo "$updated_content" > snap/snapcraft.yaml
echo Package Version="$version"

export LANGUAGE=en_US.UTF-8
export LANG=en_US.UTF-8
export LC_CTYPE=en_US.UTF-8