# Agenda & Planning Manager – Microservices Architecture

This project is a planning and agenda management system developed in C# using ASP.NET Core, structured around a microservices architecture. It allows users to manage events, join groups, and organize their schedules with fine-grained control over access and permissions.

The system is divided into two main microservices. The first handles event data, including the creation and organization of events into groups. Users can subscribe to groups to access their events, and each group has its own role system: a user can be an administrator in one group and a regular member in another. Roles determine what actions a user can perform, such as editing events or managing group members. This microservice uses a SQLite database and exposes its functionality through a dedicated API.

The second microservice manages user accounts, including authentication, password handling, and user profiles. It tracks which groups and events each user is associated with, and stores their roles and permissions. Authentication is handled securely using JWT tokens, and each user has a unique username and associated email address. This service also uses SQLite and provides a clean, intuitive API.

Both microservices are accessed through a gateway that exposes only the necessary data to the front-end. The front-end is built with Razor Pages and offers a dynamic, user-friendly interface. It includes a chronological agenda view with adaptable time slots, color-coded elements based on group type and permissions, and features for managing account settings and group memberships.

This modular design ensures scalability, security, and a clear separation of concerns, making it a robust foundation for collaborative scheduling and event management.

<img width="1919" height="943" alt="image" src="https://github.com/user-attachments/assets/336ea4c5-939e-48db-914a-ae0890121091" />

<img width="1919" height="941" alt="image" src="https://github.com/user-attachments/assets/b8de701f-a7a7-4306-8245-446bdb15b443" />
