[![Build Status](https://travis-ci.org/KonH/UnityCiPipeline.svg?branch=master)](https://travis-ci.org/KonH/UnityCiPipeline)
# UnityCiPipeline

Example of usage Unity projects in continuous integration scenarios.

## Features

- Support all available unity versions (but Linux release is required)
- Build project & upload it to itch.io
- Latest short commit hash is used as last part of the version (like 1.2.3.9ee2d1d)

## How to use

### Initial setup

- Install docker locally
- docker run -e UNITY_USERNAME={unity_account_username} -e UNITY_PASSWORD={unity_account_password} konh/unity_build_image:latest /bin/bash -c "./activate.sh"
- Save activation file content into .alf file
- Go to https://license.unity3d.com/manual, upload saved file, download license file
- ./build.sh -target=Encode-License-File -fileName={path_to_ulf}
- Save encoded content into UNITY_ULF env variable later

### Travis usage

- Create your own repo and enable it at https://travis-ci.org/
- Set up butler (https://itch.io/docs/butler/)
- Adds several required environment variables into Travis repository settings:
	- **REPO_URL** - your repository URL
	- **BUTLER_API_KEY** - see 'Running butler from CI builds' section at https://itch.io/docs/butler/login.html
	- **ITCH_TARGET** - path to your game like konh/test-ci:html
	- **UNITY_USERNAME**, **UNITY_PASSWORD** - your Unity account credentials
	- **UNITY_ULF** - base64 of your .ulf file (according to Initial setup section)
- Additional environment variables:
	- **INSTALL_ARGS** - arguments to Unity install ('Unity,WebGL' by default)
	- **BUILD_TARGET** - platform to build ('WebGL' by default)
- Push your repo changes
- See how your game is published to itch.io
