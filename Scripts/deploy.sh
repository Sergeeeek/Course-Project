#!/bin/sh

commit=$(git rev-parse --short HEAD)
project="Course-Project"

tar -zcvf $(pwd)/$(project)-windows-$(commit).tar.gz Build/windows
tar -zcvf $(pwd)/$(project)-linux-$(commit).tar.gz Build/linux
tar -zcvf $(pwd)/$(project)-osx-$(commit).tar.gz Build/osx