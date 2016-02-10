#!/bin/sh

# Example install script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# This link changes from time to time. I haven't found a reliable hosted installer package for doing regular
# installs like this. You will probably need to grab a current link from: http://unity3d.com/get-unity/download/archive
echo 'Downloading from https://github.com/github/git-lfs/releases/download/v1.1.1/git-lfs-darwin-amd64-1.1.1.tar.gz'
curl -o git-lfs.tar.gz https://github.com/github/git-lfs/releases/download/v1.1.1/git-lfs-darwin-amd64-1.1.1.tar.gz

echo 'Installing git-lfs'
tar -xzvf git-lfs.tar.gz
sudo ./git-lfs-1.1.1/install.sh
git lfs pull

echo 'Downloading from http://netstorage.unity3d.com/unity/e87ab445ead0/MacEditorInstaller/Unity-5.3.2f1.pkg: '
curl -o Unity.pkg http://netstorage.unity3d.com/unity/e87ab445ead0/MacEditorInstaller/Unity-5.3.2f1.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /