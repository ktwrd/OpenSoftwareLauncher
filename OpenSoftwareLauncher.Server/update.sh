#!/bin/bash

sudo systemctl stop buildapi.service

rm -rf osltmp
mkdir osltmp
cd osltmp

wget $1
unzip *.zip
rm *.zip

cd ..


mv OpenSoftwareLauncher.Server OpenSoftwareLauncher.Server.old
rm osltmp/appsettings.*
cp -rf osltmp/* ./

sudo systemctl start buildapi.service
