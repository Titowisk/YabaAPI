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

- deploy the application to a Cloud (Google, Azure, Aws...)
- Monitor the cluster using prometheus + grafana ?
- Add rabbitMQ example

## Kubernetes

- Create a kubernetes cluster [OK]
- Deploy the api to the cluster using Deployment [OK]
- Deploy the database to the cluster using StatefulSet [OK]
- Deploy worker to the cluster using Deployment
	- Use another Pod ? Multiple container Pod ?
- Deploy rabbitMQ to the cluster using ?
- Add otel colector to the cluster

## Continuous Integration
- new Pull Requests should trigger the application unit tests
- closing Pull Requests should trigger a new docker image push [OK]

- new exchanges/topics/queues should ?

## Continuous Delivery
- a successfull image push should trigger a deployment to the kubernetes cluster (ArgoCD) [OK]
	- the deployment is done by changing the application.yaml tracked by ArgoCD [OK]

# ToDo (Currently Working On)
- Try to deploy kafka components (kafka-cp, zookerper, kafka-ui) and yaba worker as it is to the minikube local cluster. 
	- see docker-compose.yaml for reference
- Remember how to apply the application.yaml (ArgoCD) manually to make repeated tests until a successfull deployment
- Check with Postman and kafka ui if everything is working inside the cluster

## 01/04/2025
- I learned how to create different applications in a cluster inside minikube using argoCD
- But my kafka test is not working yet, ArgoCD is complaining about something that I couldn't check because
the client is throwing errors. Minikube is buggy.

## 31/03/2025
- I made changes that now need to be tested. I asked chatGPT to help me setting up what is needed.
- Now I need to test the first option to see if works like I want
