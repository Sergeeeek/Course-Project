#!/bin/sh

# Example install script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# This link changes from time to time. I haven't found a reliable hosted installer package for doing regular
# installs like this. You will probably need to grab a current link from: http://unity3d.com/get-unity/download/archive
echo 'Installing git-lfs'
curl -s https://packagecloud.io/install/repositories/github/git-lfs/script.deb.sh | sudo bash

sudo apt-get install git-lfs

git lfs install
git lfs pull

echo 'Downloading from http://download.unity3d.com/download_unity/linux/unity-editor-5.3.2f1+20160208_amd64.deb'
curl -o unity.deb http://download.unity3d.com/download_unity/linux/unity-editor-5.3.2f1+20160208_amd64.deb

echo 'Installing Unity'
sudo dpkg -i unity.deb
echo 'Fixing dependencies'
sudo apt-get -f install