#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="Course-Project"

echo "Attempting to build $project for Windows"
/opt/Unity/Editor/Unity \
  -batchmode \
  -silent-crashes \
  -logFile $(pwd)/unitywin.log \
  -projectPath $(pwd) \
  -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
  -quit

echo "Attempting to build $project for OS X"
/opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unityosx.log \
  -projectPath $(pwd) \
  -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
  -quit

echo "Attempting to build $project for Linux"
/opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unitylin.log \
  -projectPath $(pwd) \
  -buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe" \
  -quit

echo 'Logs from build'
cat $(pwd)/unitywin.log
cat $(pwd)/unityosx.log
cat $(pwd)/unitylin.log