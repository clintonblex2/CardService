# Project Description

The Cards application is a task management system that allows users to create and manage tasks in the form of cards. Users are uniquely identified by their email addresses and have roles (Member or Admin) for accessing and managing cards. Authentication is performed using JSON Web Tokens (JWT), ensuring secure access to the application.

## Key Features

- **User Authentication:** Users can authenticate themselves using their email address and password to access the application.
- **Card Management:** Users can create, update, and delete cards. Each card consists of a name, description, color, and status (To Do, In Progress, or Done).
- **Access Control:** Members have access to cards they created, while admins have access to all cards, ensuring appropriate data privacy and security.
- **Search and Filtering:** Users can search for cards based on various criteria such as name, color, status, and date of creation. Optional filtering, sorting, and pagination options are available to streamline the search process.
- **RESTful API:** The application provides a RESTful API that allows seamless integration with other systems. The API supports CRUD operations for managing cards, as well as search and filtering capabilities.

The Cards application aims to provide a user-friendly and efficient solution for managing tasks.
