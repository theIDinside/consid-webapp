# Consid Web App technical assessment

Used technologies

- SDK Version: 6.0
- C# Version >= 8.0
- Entity Framework 6.0
- ASP.Net Core MVC

We use .NET Core for it's cross platform capabilities and also because the actual web app,
runs in user space as a "regular" program, while the ASP.NET Framework version, runs using IIS
which makes it harder to actually understand what it's doing, if you have no prior knowledge or understanding of IIS.

# Usage

There are some requirements to get this application running. These include installing 3rd party libraries (frameworks/packages), however, the dotnet cli should handle that for you automatically.
If not, go to the workspace folder and type

```bash
    dotnet restore
```

However the documentation says that when `dotnet build` or `dotnet run` is executed, it runs `dotnet restore` so there should be no issues there.

On Linux, this application is easy to get going. You will have had installed mssql-server along with it's tooling mssql-tools, docs for installing on Ubuntu can be found [here](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-ubuntu?view=sql-server-ver15). Once you've install mssql-server, install mssql-tools for the command line utilities required to connect to the database.

If you already have an sql server set up, the connection string exists in the appsettings.Development.json file. You'll need to set it to whatever password and user you can identify/authenticate with. In this project, I've used a default user "sa" and the password "Consid2021".

On linux, if the user ID and password is OK and you can connect to the server, the database and the tables will be created automatically by Entity Framework 6.0.

## 3rd party libraries (packages)

- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

This application uses Microsoft's "Entity framework", in an ASP.NET MVC application.
