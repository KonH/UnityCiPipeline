# UnityCiPipeline

Example of usage Unity projects in continuous integration scenarios.

## Features

- Support all available unity versions
- Build project & upload it to itch.io
- Latest short commit hash is used as last part of the version (like 1.2.3.9ee2d1d)

## How to use

- Perform Unity manual activation and save .ulf file
- Run ./build.sh -target=Encode-License-File and save base64 value
- Create your own repo and enable it at https://travis-ci.org/
- Set up butler (https://itch.io/docs/butler/)
- Adds two environment variables into Travis repository settings:
	- **BUTLER_API_KEY** (see 'Running butler from CI builds' section at https://itch.io/docs/butler/login.html)
	- **ITCH_TARGET** (path to your game like konh/test-ci:html)
	- **UNITY_USERNAME** (your Unity account credentials)
	- **UNITY_PASSWORD** (your Unity account credentials)
	- **UNITY_ULF** (base64 of your .ulf file)
- Push your repo changes
- See how your game is published to itch.io

## License workaround

This approach potentially not working due to license issues, you can try to add serial to Unity start command, if you use Plus/Pro subscription.