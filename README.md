# verification-api
Face verification web API, developed using ASP.NET web API in Visual Studio 2022.

To use this project, ensure that you have three things in your PC.
- Deepface files (you can download it using pip install in anaconda or https://github.com/serengil/deepface)
- .NET 8.0, Visual Studio 2022
- SQL Server Management Studio

If there's any URL that uses IP address inside my codes, please change it according to your IP address.

Inside the project, there are basic CRUD endpoints and a verification endpoints.

# Verification endpoint:
- When you already installed deepface, go to the folder that contains deepface and find its API file. After that,
you can open anaconda prompt and cd the path that contains deepface API file. after that, run the API file using anaconda prompt.
- After running the API, there will be 2 URLs that will be displayed. One is for localhost (only you can access) and another one
is for your network (if you use the second URL, all devices from your network can access deepface).
- Don't forget to change the URLs.
- Also, don't forget to change the connection string in "appsettings.json".

If you have any inquiry, feel free to pull issues.
