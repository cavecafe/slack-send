#!/bin/bash

#########################################################
# Increase Build Number in AssemblyInfo.cs file
# This script increases the build number in the AssemblyInfo.cs file
# by 1. The AssemblyInfo.cs file is located in the Properties folder
perl -i -pe 's/(BuildNumber\(&quot;)(\d+)(&quot;\))/ $1.($2+1).$3 /e' AssemblyInfo.cs
echo "Build #"| grep -oPm1 "(?<=BuildNumber\(&quot;)[^&]+(?=&quot;\))" AssemblyInfo.cs

#########################################################
# Replace the Package version in the snapcraft.yaml file 
# with the value in the project file
version=$(grep -oPm1 "(?<=<Version>)[^<]+" slack-send.csproj)
formatted_version="'$version'"
snapcraft_content=$(cat snap/snapcraft.yaml)
updated_content=$(echo "$snapcraft_content" | sed "s/version: .*/version: $formatted_version/")
echo "$updated_content" > snap/snapcraft.yaml
echo Package Version="$version"

export LANGUAGE=en_US.UTF-8
export LANG=en_US.UTF-8
export LC_CTYPE=en_US.UTF-8