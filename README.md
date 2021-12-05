# Web App technical assessment

Used technologies

- .NET Core SDK Version: 6.0
- C# Version >= 8.0
- Entity Framework 6.0
- ASP.Net Core MVC
- MSSQL Server

The justification for the use of .NET Core is it's cross platform capabilities and also because the actual web app,
runs in user space as a "regular" program, while the ASP.NET Framework version, runs using IIS
which makes it harder to actually understand what it's doing, if you have no prior knowledge or understanding of IIS.

The reason why I'm not using SQLLite for instance as the SQL "server" is because SQLLite does not support Async as far as I know.

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

The above steps seem to work just fine on Windows as well.

If you're using VSCode as a development environment, starting this should be fairly easy. Make sure you have the Omnisharp extension installed and press F5, if no configuration file exists in the .vscode/ folder, it will create one for you. It will download 3rd party libraries, build and run and the webapp will listen on `localhost:5182`. From there, you should be able to navigate to all the features laid out in the requirements document.

## 3rd party libraries (packages)

- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools