# WEB APPLICATION “COSMOS ODYSSEY”
Link to ASP.NET Core MVC web application hosted in Azure: `https://cosmos-odysseyapp.azurewebsites.net`

## Prerequisites
- .NET 7
- PostgreSQL database
- Docker, docker-compose

## Setup guide (how to run the code)
1. Clone the project `https://gitlab.cs.taltech.ee/dakurb/cosmosodysseyapp.git`
2. Install Docker, if not yet. Link to installation guide `https://docs.docker.com/compose/install/` (recommendation: install Docker Desktop, which includes all necessary prerequisites, like docker-compose etc.) 
3. Checkout that docker-compose is installed.
4. After successful installation, move to project's root directory.
5. Run `docker compose up` command, which will start both: web application and database.
6. Now the application should be accessible on `http://localhost:8001/` url.

