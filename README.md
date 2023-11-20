# GambitChallenge - fullstack
## Table of Contents
1. [Introduction](#intro)
2. [Feature](#feats)
3. [Technologies](#tech)
4. [Usage](#use)
5. [Authors](#aut)
6. [Sources](#sou)

## 1. Introduction
This is dating app website (demo)
this includes both front end client and serverside
I also included Automated deployment for this app so basic DevOps
between Github and fly.IO, using git actions

* Data is formatted to int / long or Real4 as given in the clues
* Negative values are give - symbol
* Units are displayed to values that have relatively readable proper Unit value
* this should improve readability

###live app at
* https://gambitapp.fly.dev/
* you can register or using dummy data:
* username: lisa
* password: Pa$$w0rd


if you want try on container then pull from here: kouzber/datingapp:latest
## 2. Features
* Creates a database for users
* validate authentication
* Api endpoints
* Client side validation
* Loading spinner
* SignalR chat communication
* Like other user and match with them

## 3. Technologies
* ASP.NET
* .NET
* C#
* SSMS
* AUTH > microsoft packages
* Angular
* Git action
* Docker

## 4. Usage

### Tools ###
As an editor, you can use Visual Studio Code or Visual Studio.
If you are using visual studio download packages ASP.NET and web development, .NET desktop development and
Data storage and processing with it.

In order to run dotnet commands you need .NET SDK for you machine..

### Setup ###
docker run --name dev -e POSTGRES_USER=appuser -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:latest
Change your connection string in app.config file.

```javascript
<?xml version="1.0" encoding="utf-8"?>
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=appuser; Password=secret;Database=gambit"
  },
//for container
"DefaultConnection": "Server=host.docker.internal;Port=5432;User Id=appuser; Password=secret;Database=gambit"
```
In the terminal run:
### `dotnet watch run`
and terminate it with CTRL + C. This runs all the nuget packages needed for  the project.


In the terminal download dotnet tools with the command:
### `dotnet tool install --global dotnet-ef`

Then write in the terminal:
### `dotnet ef database update`
This creates the tables.

Now that the tables are done, run again
### `dotnet watch run`
and terminate it with crtl + C. While it's shutting down it imports the data. Because of the size of the files this might take several minutes. You can stop it early with ctrl + C again. It will still put some of the data in the tables, which is enough for trying the program.

### `docker run`
docker run -rm -it -p 8080:80 kouzber/datingapp

Now put it running again with:
### `dotnet watch run`
as for client side
### `npm i`
### `npm start`

### View ###
While the program is running you can see the api at: https://localhost:{your port}/swagger/index.html
While both backend and frontend are running you can view the application at: http://localhost:3000/
or localhost:8080
## 5. Authors
[@Tom N](https://github.com/kouzber123)


## 6. Sources
* https://learn.microsoft.com/en-us/nuget/
* https://fly.io/docs/
* https://docs.docker.com/
