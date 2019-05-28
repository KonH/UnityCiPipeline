#!/usr/bin/env bash

UNITY_PATH=$1       # path to Unity executable (.../Unity.app/Contents/MacOS/Unity)
ITCH_DESTINATION=$2 # itch.io target like userName/projectName:platform
VERSION=$3          # application version

echo "Remove old Build..."
rm -rf Build

echo "Run Unity from $UNITY_PATH..."
${UNITY_PATH} -quit -batchmode -logFile - -executeMethod UnityCiPipeline.CustomBuildPipeline.RunBuildForVersion -projectPath "$(PWD)" -version=${VERSION}

echo "Push to itch.io..."
butler push --userversion=${VERSION} --verbose Build ${ITCH_DESTINATION}