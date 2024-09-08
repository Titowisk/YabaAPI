# Introduction
This project works as a playground to experiment new features, refactorings and any new stuff I find
interesting on the internet

## Application
The application is a CSV bank statement manager. It reads bank statements for different banks and saves them for each user's bank account.
The user can then categorize this transaction to better help organize their life finances.

### Asynchronous Categorizing
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
- https://www.rabbitmq.com/docs/management
- https://www.rabbitmq.com/tutorials/tutorial-two-dotnet
- https://hub.docker.com/_/rabbitmq
- https://github.com/MassTransit/MassTransit?tab=readme-ov-file
- 

## Technologies
This is the list of technologies I already experimented in this application

- Bogus: to generate random information for my entities
- Docker: to learn how to run my application and external dependencies entirely using containers
- SQL Server
- Net8: new framework features
- Messaging: azure service bus
- Webjob: for asynchronous work
- XUnit: for tests

## naming convention

- API:
	- snake-case
	- REST

# How to Use

## Requirements
- Docker Desktop (on Windows)
- Knowledge on APIs
- Knowledge on SQL
- Postman or a similar software

##  Getting Started
- git clone branch master
- go to solution folder
- run command `docker compose up`
	- The api is configured to listen to http://localhost:5000 and swagger using http://localhost:5000/swagger/index.html

This api was configured to seed a user, a bank account and 100 transactions on startup
You can log in using: 
- email: test-seed@gmail.com
- password: 123Correct_

### Debug
- run the sql server container
`docker run -d -it -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=A&VeryComplex123Password" -p 1433:1433 --name sql-server-2022-debug mcr.microsoft.com/mssql/server:2022-latest`
- change appsettings.Development.json
Replace "Server=yabaapi-sql-server-2022-1" with "Server=localhost"
- run rabbitMQ container
`TODO`
- run api and worker projects using Visual Studio

# Notes
- The webjob and the messaging part is not working yet because of recent changes I had to make in the entire solution
- The csv input reading is not working for the same reason