# BooksInternalService
Service to fetch books volume information from Google Books API
Setting up And Running the WebApi Application

# Components
Books.Service.Internal.Api : Webapi Project
Books.Serfvice.Api.Tests: Test Project
Log File for Analytics

# Prerequisites
VS or VS Code
SQL Server instance running on local machine or docker with username and password

# Running Webapi

Navigate to Books.Service.Internal.Api/appsettings.json and edit the "ConnectionStrings" value to have userID and password of sql server or if you are running SQL server on windows,
use the alternate option of integrated security in commented line
Choose Books.Service.Internal.Api as Startup Project.
Open package manager console and Run the command "dotnet ef database update"
Open SQL server and check if a new database "ReadersDB" has been created and TblRoles has data created for "admin" and "demo"
If previous step is success, run command "dotnet run" in console or VS Debug to proceed to run the application on IISExpress.
On running application, the swagger webpage will be opened up with url "https://localhost:7288/swagger/index.html"
On this swagger page, find the "api/User/Register" post endpoint api (or use postman to POST request to "https://localhost:7288/swagger/index.html") and
use the request body shown below.  This will create user with credentials successfully
{
  "userid": "test2",
  "name": "test2",
  "password": "test",
  "email": "test@test.co",
  "role": "admin"
}
Once the user creation is success, we had to obtain jwt token from Authenticate endpoint to gain access to internal books endpoint. Use same swagger page or use postman or any request client to send POST request
to "https://localhost:7288/api/User/authenticate" with request body shown below:
{
    "username":"test2",
    "password":"test"
}
On getting the jwttoken as response, copy the value of "jwttoken".
To submit request to the objective service viz the internal books api, we have to use this copied "jwttoken" as the endpoint is only available for authenticated and authorized users.  So, its secured and we cannot use swagger.
Open postman or any client of choice and create a GET request for books api url "https://localhost:7288/api/Internal/books/prod?pageIndex=1"
with mandatory header for "Authorization" with value as "Bearer <copiedjwttoken>".
We should also send additional headers "X-Session-ID" (string value) and "X-User-ID" (string value) for logging purposes

# Test Project
Test project has functional tests for books api validating the critical aspects of functionality.

# Deployment
We could setup pipeline to makesure it deploys conditionaly on results of TestProject's test runs and deploy on commits

# Logging
Log files for analytic purposes is generated as a text file (could be consumed by Splunk) on a Day frequency setup and its availble in
the root folder of application  "~/Logs/InternalApiLogYYYYMMDD.log".

