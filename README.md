# Project MyApp

## Prerequisites
- Docker and Docker Compose installed
- .NET 8 SDK installed


## Deploy MongoDB with Docker

```bash
docker run -d --name mongodb -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=admin123 mongo:6.0
```

## Run the Application
1. Open a terminal in the project root directory.
2. Navigate to the EntryPoint directory:

   ```powershell
   cd src/EntryPoint
   ```
3. Run the application:

   ```powershell
   dotnet run
   ```
## Add documents to MongoDB
The JSON examples in `db.md` can be used to insert documents into a MongoDB database.
