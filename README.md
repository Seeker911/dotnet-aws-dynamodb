# dotnet-aws-dynamodb
This project is used to test and explore the interface to AWS DynamoDb from a dotnet project. 

The project has been tested on Ubuntu 20.04 and AWS Linux2. I have included the systemd service config file

It should in no way be considered to make use of best practises nor suggest this is the best way of doing things

Use entirely at your own risk

I have used a local version of DynamoDb in a docker container. See below.

docker run --name awsdynamodbcontainer -d -v e:/deleteme/data:/home/dynamodblocal/data -p 8000:8000 amazon/dynamodb-local -D'java.library.path=./DynamoDBLocal_lib' -jar DynamoDBLocal.jar -sharedDb -dbPath ./data

OR use the docker-compose file: docker-compose.yml