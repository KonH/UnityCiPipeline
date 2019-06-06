./clone.sh
./install.sh
cd /project
./build.sh -target=Decode-License-File
./build.sh -target=Use-Manual-Activation-File
./build.sh -target=Build
./build.sh -target=Upload