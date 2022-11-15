#!/bin/bash

alias rm="/bin/rm"

sudo systemctl stop buildapi.service

rm -rf osltmp
mkdir osltmp
cd osltmp

wget $1
unzip *.zip
rm *.zip

cd ..

rm -f OpenSoftwareLauncher.Server.old
rm -f OpenSoftwareLauncher.Server.Old
mv OpenSoftwareLauncher.Server OpenSoftwareLauncher.Server.Old
rm osltmp/appsettings.*
cp -rf osltmp/* ./
rm -rf osltmp

sudo systemctl start buildapi.service
