#!/usr/bin/env bash

docker run \
	-e REPO_URL \
	-e UNITY_USERNAME -e UNITY_PASSWORD -e UNITY_ULF \
	-e BUTLER_API_KEY -e ITCH_TARGET \
	-e INSTALL_ARGS -e BUILD_TARGET \
	konh/unity_build_image:latest \
	/bin/bash -c "chmod +x /build.sh && ./build.sh"