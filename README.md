# 🦡 Honeybadger CMS

A modern, headless content management system built with .NET 9, providing enterprise-grade content management through a GraphQL API with dynamic content type support.

## Overview

Honeybadger CMS is designed for developers and organizations who need a flexible, scalable content management solution. Built with an API-first approach, it enables seamless integration with any frontend technology while providing robust content modeling capabilities through dynamic schema generation.

## 🚀 Features

**Headless Architecture**  
API-first design powered by GraphQL (HotChocolate) for maximum flexibility and integration capabilities.

**Dynamic Content Types**  
Create and manage custom content types programmatically with automatic database schema generation and validation.

**Enterprise Database Support**  
PostgreSQL-backed storage with dynamic table creation, ensuring data integrity and performance at scale.

**Modern Technology Stack**  
Built on .NET 9 with contemporary development practices, ensuring maintainability and performance.

**Type Safety**  
Strongly typed content management with comprehensive GraphQL schema validation and error handling.

## 🏗️ Architecture

The solution is structured as follows:

- **Honeybadger.Api**: GraphQL API layer with schema configuration and endpoint management

## 🛠️ Prerequisites

- .NET 9 SDK or later
- PostgreSQL 15+ database server
- Development environment: Visual Studio 2022, JetBrains Rider, or VS Code

## ⚙️ Installation & Setup

### 1. Repository Setup

```bash
git clone https://github.com/yourusername/honeybadger-cms.git
cd honeybadger-cms
```

### 2. Database Configuration

Configure your PostgreSQL connection string in `appsettings.json` or using user secrets:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=honeybadger;Username=your_username;Password=your_password"
  }
}
```

### 3. Application Launch

```bash
dotnet restore
dotnet build
dotnet run --project src/Honeybadger.Api
```

The GraphQL endpoint will be accessible at: `http://localhost:5001/graphql`

## 📝 API Usage

### Content Type Management

Content types define the structure and schema for your content entities. Each content type automatically generates a corresponding database table with the specified field definitions.

#### Creating a New Content Type

```graphql
mutation CreateBlogPostType {
  createContentType(input: {
    name: "BlogPost"
    fields: [
      { name: "Title", type: TEXT }
      { name: "Content", type: TEXT }
      { name: "Author", type: TEXT }
      { name: "PublishedDate", type: TIMESTAMP }
      { name: "IsPublished", type: BOOLEAN }
      { name: "ViewCount", type: INTEGER }
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

#### Querying Content Types

```graphql
query GetContentType {
  contentType(name: "BlogPost") {
    name
  }
}
```

### Supported Data Types

The system supports the following database data types for content fields:

| Type | Description | Use Cases |
|------|-------------|-----------|
| `TEXT` | Variable-length text | Titles, descriptions, content |
| `SMALLINT` | 16-bit integer | Small numeric values, flags |
| `INTEGER` | 32-bit integer | Counters, IDs, standard numbers |
| `BIGINT` | 64-bit integer | Large numbers, timestamps |
| `DECIMAL` | Decimal numbers | Prices, precise calculations |
| `BOOLEAN` | True/false values | Flags, switches, states |
| `TIMESTAMP` | Date and time | Creation dates, schedules |

## 📚 GraphQL Schema Reference

```graphql
schema {
  query: Query
  mutation: ContentTypeMutation
}

type AddContentTypeFieldPayload {
  name: String!
  type: DatabaseDataType!
}

type AddContentTypePayload {
  name: String!
  fields: [AddContentTypeFieldPayload!]!
  errorMessage: String!
  createdAt: DateTime
  contentTypeAdded: Boolean!
}

type ContentType {
  name: String!
}

type ContentTypeMutation {
  "Registers a new content type and creates its table."
  createContentType(input: AddContentTypeInput!): AddContentTypePayload!
    @cost(weight: "10")
}

type Query {
  hello: String!
  contentType(name: String!): ContentType!
}

input AddContentTypeFieldInput {
  name: String!
  type: DatabaseDataType!
}

input AddContentTypeInput {
  name: String!
  fields: [AddContentTypeFieldInput!]!
}

enum DatabaseDataType {
  TEXT
  SMALLINT
  INTEGER
  BIGINT
  DECIMAL
  BOOLEAN
  TIMESTAMP
}

"The purpose of the `cost` directive is to define a `weight` for GraphQL types, fields, and arguments. Static analysis can use these weights when calculating the overall cost of a query or response."
directive @cost(
  "The `weight` argument defines what value to add to the overall cost for every appearance, or possible appearance, of a type, field, argument, etc."
  weight: String!
) on SCALAR | OBJECT | FIELD_DEFINITION | ARGUMENT_DEFINITION | ENUM | INPUT_FIELD_DEFINITION

"The `@specifiedBy` directive is used within the type system definition language to provide a URL for specifying the behavior of custom scalar definitions."
directive @specifiedBy(
  "The specifiedBy URL points to a human-readable specification. This field will only read a result for scalar types."
  url: String!
) on SCALAR

"The `DateTime` scalar represents an ISO-8601 compliant date time type."
scalar DateTime @specifiedBy(url: "https://www.graphql-scalars.com/date-time")
```

## 📫 Support & Documentation

For technical support, feature requests, or bug reports, please create an issue in the project repository. We strive to respond to all issues within 48 hours.