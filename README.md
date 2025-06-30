# 🦡 Honeybadger CMS

Honeybadger CMS is a modern content management system built with .NET 9. It provides a flexible and scalable solution for managing content through a GraphQL API.

## 🚀 Features

- **Headless Architecture**: API-first approach using GraphQL
- **Dynamic Content Types**: Create and manage custom content types on the fly
- **PostgreSQL Database**: Robust and reliable data storage
- **Modern Stack**: Built with .NET 9 and modern development practices
- **Type-safe**: Strongly typed content management
- **Domain-Driven Design**: Clean and maintainable architecture

## 🏗️ Architecture

The solution follows a clean architecture pattern with the following projects:

- **Honeybadger.Api**: GraphQL API endpoints and configuration
- **Honeybadger.Application**: Application logic and use cases
- **Honeybadger.Domain**: Domain models and business logic
- **Honeybadger.Infrastructure**: Data persistence and external services

## 🛠️ Prerequisites

- .NET 9 SDK
- PostgreSQL database server
- IDE (Visual Studio 2022 or JetBrains Rider recommended)

## ⚙️ Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/honeybadger-cms.git
cd honeybadger-cms
```

2. Update the connection string in your configuration to point to your PostgreSQL database.

3. Build and run the solution:
```bash
dotnet build
dotnet run --project src/Honeybadger.Api
```

## 📝 Usage

### Creating Content Types

You can create content types using the GraphQL API. Here's an example mutation:

```graphql
mutation {
  createContentType(input: {
    name: "BlogPost"
    fields: [
      { name: "Title", type: TEXT }
      { name: "Content", type: TEXT }
      { name: "PublishedDate", type: TIMESTAMP }
    ]
  }) {
    name
    fields {
      name
      type
    }
    errorMessage
    createdAt
    contentTypeAdded
  }
}
```