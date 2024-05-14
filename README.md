# Introduction
This project works as a playground to experiment new features, refactorings and any new stuff I find
interesting on the internet

## Application
The application is a CSV bank statement manager. It reads bank statements for different banks and saves them for each user's bank account.
The user can then categorize this transaction to better help organize their life finances.

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


# Notes
- The webjob and the messaging part is not working yet because of recent changes I had to make in the entire solution
- The csv input reading is not working for the same reason