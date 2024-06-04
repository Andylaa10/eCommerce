# :computer: PBSW Exam Project - eCommerce 
**eCommerce** is a backend based software project that focuses on optimizing databases for developers, system integration and the development of large systems. The project uses the polyglot persistence principle for using multiple databases for the same system (Relational, Document-based & key-value store). The architecture is built with a focus on distributed microservices - where a CI/CD pipeline is used to validate the codebase & deploy the application via a combination of Docker hub and Azure's Kubernetes Service. Furthermore the project is focused on building a system that correlates to fault isolative architecture via the usage of Polly, in combination with monitoring via Seq and Zipkin. Finally the usage of a dead letter queue also supports the focus of developing a large scalable system
<br>

## :pencil2: Project developed by: 
- [@Andylaa10](https://github.com/Andylaa10)
- [@KristianHollaender](https://github.com/KristianHollaender)
- [@MarcusIversen](https://github.com/MarcusIversen)  
 

## :pencil: Setup steps/guide 
- **.NET8** is required for this application.
- **Docker (Desktop)** is required for composing this application.
- In `../appsettings.json` assure that the syntax is as following: 

### appsettings.json for Postgres
```
 "AppSettings": {
    "Secret": "InsertYourSecretHere",
    "AuthDB": "Host=authdb;Port=5432;Database=AuthDB;Username=postgres;Password=postgres"
  }
```
### appsettings.json for MongoDB
```
"AppSettings": {
    "CartDB": "mongodb://host.docker.internal:27018"
  }
```
## :white_check_mark:  Running the application
- Go to `../eCommerce` and do following command
```
docker compose up --build
```
- To stop application do the following command(s)
```
docker compose down
```
or
```
ctrl+c + docker compose down
```

## :open_file_folder: Databases for Developers
### Introduction
In an evolution of theories and practices regarding database management, the architectural principle of polyglot persistence has emerged. Breaking the traditional architecture of software applications, where having a homogeneous database approach, often relational, is widely used. Is this useful when developing specialized software applications with specific use cases and needs?

### Scope
For the database part of this project we implemented three different database types in the form of:
- PostgreSQL (Relational)
- MongoDB (Non-relational/Document based)
- Redis (Key-value store / caching) 

This is the architecture of the databases and it is created with focus on using the polyglot persistence principle.

 <img src="https://media.discordapp.net/attachments/1042375108494377041/1247476446738518070/image.png?ex=66602a79&is=665ed8f9&hm=10d59c93209062cf4a618740e702edc6c38a0d15c3d6379f0ebf5737f1e3968b&=&format=webp&quality=lossless&width=1374&height=1102" style="width: 500px; height: auto; border-radius: 50%;">  

## :hammer_and_wrench: System Integration 
### Introduction
When developing a more complex application for a customer or stakeholder, it is important to automatically ensure that the quality and functionality of the code are as high as possible. The increasing complexity of modern applications needs automated processes to maintain code integrity, efficiency, and reliability throughout the development process.

### Scope
For the system integration part of this project, we implemented a Continous Integration & Continuous Deployment/Delivery pipeline via Github Actions. The pipeline is responsible for validation the backend (CI), deploying dockerized images to Docker Hub (CD) & deploying application to Azure's Kubernetes Service, using the deployed Docker images (CD). 

- https://github.com/Andylaa10/eCommerce/actions

The flow of our pipeline is as following: 


 <img src="https://media.discordapp.net/attachments/1042375108494377041/1247472418457653258/image.png?ex=666026b9&is=665ed539&hm=bd45d8006f01dbc72e578094a079ae9c583be2af660df88504d81a10b34cc925&=&format=webp&quality=lossless&width=614&height=700" style="width: 450px; height: auto; border-radius: 50%;">  

## :chart_with_upwards_trend: Development of Large Systems
### Introduction (Coming soon...)
### Scope (Coming soon...)

<br>
<br>
<div style="display: flex; justify-content: center; gap: 10px;">  
    <img src="https://media.discordapp.net/attachments/1042375108494377041/1217761415792824350/Screenshot_2024-03-14_at_10.08.42.png?ex=665f8272&is=665e30f2&hm=3aa3c29ad9736662d4cb3cba4e0592e446dcbf929f450b0a061133be3d5e4e2c&=&format=webp&quality=lossless&width=1102&height=1102" style="width: 200px; height: auto; border-radius: 50%;">  
    <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlFDjuPCZsAWuOUpxWPT8TFMdEgkwEgdCKrQ&s" style="width: 200px; height: auto; border-radius: 50%;">  
</div>  

