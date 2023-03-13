This is the source code accompanying https://blog.netwatwezoeken.nl/integration-testing-with-playwright-and-testcontainers/

[The original app](https://github.com/dotnet/blazor-samples/tree/main/6.0/BlazorServerEFCoreSample) is modified for the purpose of this exmaple:
- Added MS Sql Server had been added
- Removed seeding of example data in the application

# Prerequisites
- Docker (for Desktop)
- .NET 7 installed

# Run the application

1. Run the following command to start a sql instance:
`docker run -it -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`
1. Run this application using `dotnet run` or your favorite IDE.

Notes:
This example is made purely to demonstrate how full integration testing in ASP.NET can be achieved with the help of playwright and Testcontainers. 
A real code base should contain a decent set of unit tests that at least cover the core logic of the application.
