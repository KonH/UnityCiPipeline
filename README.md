# UnityCiPipeline

Example of usage Unity projects in continuous integration scenarios.

## Stability notes

This approach not yet working due to Unity license update issues.
You can try to modify RunUnity method and include your serial, possibly it can fix this problem (but it isn't tested properly).

## Features

- Support all available unity versions
- Build project & upload it to itch.io
- Latest short commit hash is used as last part of the version (like 1.2.3.9ee2d1d)

## How to use

- Create your own repo and enable it at https://travis-ci.org/
- Set up butler (https://itch.io/docs/butler/)
- Adds two environment variables into Travis repository settings:
	- **BUTLER_API_KEY** (see 'Running butler from CI builds' section at https://itch.io/docs/butler/login.html)
	- **ITCH_TARGET** (path to your game like konh/test-ci:html)
	- **UNITY_USERNAME** (your Unity account credentials)
	- **UNITY_PASSWORD** (your Unity account credentials)
- Push your repo changes
- (Not yet working) See how your game is published to itch.io