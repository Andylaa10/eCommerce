# eCommerce - Exam Project
## :computer: Product
**eCommerce** is a backend based software project that focuses on optimizing databases, system integration and the development of large systems. The project uses the polyglot persistence principle for using multiple databases for the same system. The architecture is built with a focus on distributed microservices - where a CI/CD pipeline is used to validate the codebase & deploy the application via a combination of Docker hub and the Azure Kubernetes Service. Furthermore the project is focused on building a system that correlates to fault isolative architecture via the usage of Polly, in combination with monitoring via Seq and Zipkin. At last the usage of a dead letter queue also supports the focus on developing a large scalable system
<br>

## Setup steps/guide 
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
## Running the application
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

## Databases for Developers
In an evolution of theories and practices regarding database management, the architectural principle of polyglot persistence has emerged. Breaking the traditional architecture of software applications, where having a homogeneous database approach, often relational, is widely used. Is this useful when developing specialized software applications with specific use cases and needs?


<img src="https://media.discordapp.net/attachments/1042375108494377041/1217761415792824350/Screenshot_2024-03-14_at_10.08.42.png?ex=665f8272&is=665e30f2&hm=3aa3c29ad9736662d4cb3cba4e0592e446dcbf929f450b0a061133be3d5e4e2c&=&format=webp&quality=lossless&width=1102&height=1102" style="display: inline-block; margin: 0 auto; width: 250px; height: auto;">

