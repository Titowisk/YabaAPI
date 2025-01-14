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

### Option 1 - Docker
- git clone branch master
- go to solution folder
- run command `docker compose up`
	- The api is configured to listen to http://localhost:5000 and swagger using http://localhost:5000/swagger/index.html
### Option 2 - Kubernetes
- check yaba-api.yaml for instructions

This api was configured to seed a user, a bank account and 100 transactions on startup
You can log in using: 
- email: test-seed@gmail.com
- password: 123Correct_


# Notes
- The webjob and the messaging part is not working yet because of recent changes I had to make in the entire solution
- The csv input reading is not working for the same reason

# Roadmaps

## Kubernetes

- Create a kubernetes cluster
- Deploy the api to the cluster using Deployment
- Deploy the database to the cluster using StatefulSet
- Deploy worker to the cluster using Deployment
	- Use another Pod ? Multiple container Pod ?
- Deploy rabbitMQ to the cluster using ?
- Add otel colector to the cluster
- Monitor the cluster using prometheus + grafana

## Continuous Integration
- changes to api should trigger a build
- changes to worker should trigger a build
- changes to database should ?
- new exchanges/topics/queues should ?

## Continuous Delivery