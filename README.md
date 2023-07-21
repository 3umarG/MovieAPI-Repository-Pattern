# Movies API
Simple Api build be .Net Core as Backend Service .
## Description
The Movies API is a powerful and flexible .NET 6 application that serves as a backend for managing movie-related data. It provides a comprehensive platform for storing, retrieving, creating, updating, and deleting movie information, along with associated actors and genres. Whether you're a movie enthusiast or a developer looking to enhance your skills, this API offers an ideal solution for organizing and accessing movie data.

## Key Features:

- **Efficient Data Management**: The API leverages the latest advancements in .NET 6 and EF Core, ensuring optimal performance and efficient handling of movie-related information. With LINQ support, complex queries become a breeze, enabling seamless retrieval and manipulation of data.

- **Structured Approach**: The project incorporates the Repository Pattern and Unit of Work to maintain a clean separation of concerns. This architectural design allows for modular development and facilitates easier testing, debugging, and maintenance.

- **Reliable Testing**: To ensure the robustness and reliability of the API, comprehensive unit tests have been implemented using XUnit and the FakeItEasy mocking library. These tests cover most of the controllers and actions, enabling you to confidently make changes and refactor code without fear of introducing regressions.

- **Custom Response Handling**: The Factory Method Design Pattern is employed to generate custom responses for both success and failure states of each API endpoint. By adopting this approach, the API delivers consistent and meaningful responses to clients, improving the overall user experience.

- **Seamless Object Mapping**: AutoMapper plays a vital role in simplifying the mapping process between domain models (entities) and Data Transfer Objects (DTOs). With this library, you can effortlessly transform data between different representations, reducing the amount of manual mapping code and increasing development productivity.

- **Authentication and Authorization Based** : Using JWT tool to create Access Tokens to keep our end points safe from anonymous users .

- **User Management** : Using IdentityFramework with EF Core to store users and keeping them for login again , all this working with JWT Tokens to ensure Security for our API .
  
## Technologies Used

- **.NET 6**: Framework used to develop the API.
- **EF Core**: Entity Framework Core for database access, using LINQ.
- **Unit Tests**: Describe how you implemented unit tests for most of the controllers and actions.
- **XUnit**: Testing framework used for unit testing.
- **FakeItEasy**: Mocking library used for testing.
- **AutoMapper**: Used for mapping between domain models (entities) and DTOs.
- **IdentityFramework** : For User management .
- **JWT** : To create access tokens to ensure and apply security for API .

## Installation 
To set up and run the project locally, follow these steps:

 1- Clone the repository. <br>
 2- Install the required dependencies by running ``` dotnet restore ``` in the project's root directory. <br>
 3- Configure the database connection in the ``` appsettings.json file ```. <br>
 4- Apply the database migrations using the command ``` dotnet ef database update ```. <br>
 5- Run the API using the command  ``` dotnet run ```. <br>

## Usage

To use the Movies API, you can make HTTP requests to various endpoints.
The API supports operations such as retrieving movies, creating new movies, updating movie information, deleting movies, and more. <br>


## Examples
### Auth Controller :
**login request**
```
POST /api/Auth/login

Body :
{
    "emailOrUserName": "abc@example.com",
    "password": "*****************"
}
```
**login response**
```
{
    "status": true,
    "statusCode": 200,
    "message": null,
    "data": {
        "userName": "abc@example.com",
        "email": "abc@example.com",
        "message": null,
        "isAuthed": true,
        "token": "***************************",
        "accessTokenExpiration": "2023-07-20T18:10:48Z",
        "refreshTokenExpiration": "2023-07-30T18:09:48.7634758Z"
    }
}
```

**refresh-token request**
```
GET /api/Auth/refresh-token
Cookie: refreshToken=**************************
```

**refresh-token response**
```
UnAuthorized (401)
{
    "statusCode": 401,
    "message": "Inactive Refresh Token",
    "status": false
}

Success (200)
{
    "statusCode": 200,
    "message": null,
    "status": true
}
```
## Database ERD

....

Feel free to customize this template according to your project's specific needs. Make sure to provide clear and concise explanations to help potential employers or collaborators understand your project and the technologies you used.
