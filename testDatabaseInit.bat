@echo off
echo Creating database with name of "mongodb-osl"
echo Username: user
echo Password: password
docker run -p 27017:27017 --name mongodb-osl -d -e MONGO_INITDB_ROOT_USERNAME=user -e MONGO_INITDB_ROOT_PASSWORD=password mongo