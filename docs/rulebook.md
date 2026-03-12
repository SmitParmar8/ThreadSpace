ThreadSpace – Development Rulebook

This rulebook defines the coding rules for building the ThreadSpace Developer Discussion Platform. All generated code must follow these guidelines to keep the project simple, consistent, and easy to maintain.

General Principles

Keep the implementation simple and minimal. The project is an academic prototype and does not require production-level architecture or advanced optimizations.

Avoid unnecessary abstraction layers, excessive design patterns, or overly complex folder structures.

Do not generate enterprise-level code, microservices, caching systems, or distributed architectures.

Code Structure

Use a simple and clean project structure:

Controllers
Models
DTOs
Data (DbContext)

Services should only be created if absolutely necessary.

Avoid unnecessary helper classes or additional layers.

Database Rules

Use PostgreSQL as the database.

Use Entity Framework Core as the ORM.

Always follow the provided database schema when creating models and relationships.

Do not modify the schema unless explicitly instructed.

API Route Rules

Before creating any new API route:

1. Refer to this rulebook.
2. Check existing API routes to avoid duplication.
3. Follow REST-style naming conventions.

Route naming should remain simple and consistent.

Example format:

GET /posts
POST /posts
PUT /posts/{id}
DELETE /posts/{id}

Authentication routes:

POST /auth/register
POST /auth/login

When generating any new route, ensure it follows the same pattern.

Coding Style

Do not create overly large functions.

Keep controllers short and readable.

Avoid unnecessary comments or verbose explanations in code.

Avoid complex patterns such as CQRS, event sourcing, or repository-heavy abstractions.

Do not write top-level logic blocks or complicated startup logic.

Frontend Interaction

The frontend uses simple HTML, CSS, and Vanilla JavaScript.

Data should be fetched using simple fetch API calls.

All API responses must return JSON.

Example pattern:

GET /posts returns a JSON list of posts.

Do not introduce complex frontend frameworks unless explicitly requested.

Code Complexity

Generated code must remain minimal and suitable for a prototype.

Avoid production-level logging systems, monitoring tools, distributed caching, or infrastructure complexity.

Focus only on the functionality required for the ThreadSpace platform.

Final Rule

Whenever generating new code, controllers, or routes for this project, always refer to this rulebook and follow the same structure and simplicity guidelines.
