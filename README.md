UserManagementSystem Application

Description:
- This is my first personal project developed using MVC5.
- User Management System with authentication features, implemented with Forms Authentication for login and role management.

Features:
- Role-based Display:
  - The user interface changes based on the user role. Admins have access to the Dashboard, where they can perform CRUD operations on UserLogins and UserRoles to manage users and roles.
- Authentication:
  - Users can log in, create an account, and recover their password via email, with Forms Authentication managing the authentication process.
- Account Access:
  - Once authenticated, users can access their account and profile data, with the display adapting based on their authenticated status.
- Error Handling:
  - Comprehensive error handling provides clear, personalized messages for unexpected issues.

Technical Specifications:
- IDE: Visual Studio 2022
- Framework: .NET Framework 4.7.2
- Project Structure: 
  - Class Library (DAL) and MVC5 (Client-side)
- Languages and Technologies:
  - Server-side:
    - Backend: C#, Entity Framework 6 (Code-First);
  - Client-side:
    - HTML5, CSS3, Razor Views, Bootstrap 5, JavaScript, Jquery 3.7.0
- Architecture: Object-Oriented Programming (OOP)
- Database: SQL Server 2019

How to Use:
1) Clone the Repository;
2) Set Up the Database:
  - Open the project in Visual Studio;
  - In the Package Manager Console:
    - Select "UserManagement.DAL" as the default project;
    - Type the following command:
      - update-database
3) Run the Application.
