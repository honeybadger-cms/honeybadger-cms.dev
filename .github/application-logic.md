# 🧱 CMS MVP: System Design & Behavior

## 🚨 DDD & Clean Architecture: ContentType Creation (2025-06-29)

### Domain Model

- **ContentType** is a first-class domain entity representing a content type in the CMS.
- **ContentTypeName** is a value object that encapsulates all validation and rules for content type names (e.g., regex, allowed characters).
- The domain layer exposes an interface `IContentTypeRepository` for business-oriented operations (e.g., checking existence, registering a new type).

### Content Type Creation Flow

1. **API Layer**
   - Receives a GraphQL mutation for content type creation.
   - Calls the Application layer use case/service.
2. **Application Layer**
   - Accepts a command (e.g., `RegisterContentTypeCommand`) with the content type name.
   - Uses the `ContentTypeName` value object for validation.
   - Orchestrates the registration process via `IContentTypeRepository`.
   - Returns a rich result object (`RegisterContentTypeResult`) with status, error messages, and metadata.
3. **Domain Layer**
   - Contains the `ContentType` entity and `ContentTypeName` value object.
   - All business rules and invariants are enforced here.
4. **Infrastructure Layer**
   - Implements `IContentTypeRepository` using Dapper/Npgsql.
   - Handles actual database operations (table creation, metadata storage).
5. **Testing**
   - Unit tests for Application layer (use case/service) using xUnit v3 and FakeItEasy.
   - Integration tests for Infrastructure and API layers using Testcontainers.

#### Example: RegisterContentTypeResult

```csharp
public sealed class RegisterContentTypeResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public ContentType? ContentType { get; }
}
```

#### Metadata and Table Creation

- When a new content type is registered:
  1. The system validates the name using `ContentTypeName`.
  2. Checks for existence via the repository.
  3. If valid and not existing, creates a new entry in the internal metadata table (`cms_content_types`).
  4. Creates the physical table in Postgres.
  5. Returns a result object with status and details.

#### Testing Approach

- **Unit tests**: Cover all valid/invalid scenarios for the use case/service.
- **Integration tests**: Validate repository and API integration, including actual table creation in a test database.

#### Continuous Documentation

- All changes to the content type creation flow, business rules, or architecture are reflected here before any code is written.

---

This CMS is designed with a database-first philosophy: the PostgreSQL schema is the source of truth. Content types are backed by real Postgres tables, and content items are stored as rows within those tables. The system dynamically exposes a GraphQL API generated from the actual database schema.

There is no GUI, no branching, no permissions — just the foundational mechanics for modeling and managing structured content programmatically via APIs.

## 🧩 Content Modeling

### Content Types → Tables

A content type (like Article, Product, or FAQ) corresponds to a real table in the database. When a content type is created, the system:

1. Stores metadata about the type (its name, ID, and table name) in internal CMS tables.

2. Creates a physical table in Postgres with a standard structure:

   - `id UUID PRIMARY KEY DEFAULT gen_random_uuid()`

   - `created_at TIMESTAMP DEFAULT NOW()`

   - `updated_at TIMESTAMP DEFAULT NOW()`

   - One column per user-defined field.

Example:

Creating a content type called Article with fields title and published_at results in:

```sql
CREATE TABLE article (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW(),
  title TEXT NOT NULL,
  published_at TIMESTAMP NULL
);
```

The internal metadata stores that this content type exists and tracks its fields.

### Fields → Table Columns

Each field added to a content type becomes a column on the underlying table. Fields can be:

- text
- integer
- boolean
- timestamp
- (other scalar types)

Removing a field results in a column being dropped from the table. There is no support yet for renaming fields or changing their types (those will require migration strategies).

## 🧾 Content Items → Table Rows

Content items are simply rows in the Postgres tables behind each content type.
To insert, update, or delete a content item, the system runs the appropriate SQL against the real table (`INSERT`, `UPDATE`, `DELETE`). Every content item has the same core columns (`id`, `created_at`, `updated_at`), plus user-defined fields.

Reading items is done by querying the table directly using a dynamically generated GraphQL resolver.

## 🔍 GraphQL API

The GraphQL schema is dynamically built based on the actual content types and their fields in the database.

For example, if you have a content type `Article`, the following types and operations will exist in the GraphQL schema:

```graphql
type Article {
  id: UUID!
  title: String!
  published_at: DateTime
  created_at: DateTime!
  updated_at: DateTime!
}

type Query {
  articles: [Article!]!
  article(id: UUID!): Article
}

type Mutation {
  createArticle(data: ArticleInput!): Article
  updateArticle(id: UUID!, data: ArticleInput!): Article
  deleteArticle(id: UUID!): Boolean
}

input ArticleInput {
  title: String!
  published_at: DateTime
}
```

The schema is introspected and regenerated whenever the content model changes.

## 🗃 Metadata System

The CMS uses internal tables to track content model metadata:

`cms_content_types`
Tracks high-level info about each content type.
| Column | Description |
| ----------- | ---------------------------------- |
| id | UUID |
| name | Human-friendly name (e.g. Article) |
| table_name | Snake_case table name in DB |
| created_at | Timestamp |

`cms_fields`
Tracks individual fields per content type.
| Column | Description |
| ----------------- | ----------------------------- |
| id | UUID |
| content_type_id | FK to `cms_content_types` |
| name | Field name (e.g. "title") |
| column_type | Data type (e.g. "text") |
| nullable | Whether the field is nullable |
| created_at | Timestamp |

## 💡Examples

### 🔧 1. createContentType

Create a new content type, backed by a Postgres table.

```graphql
mutation {
  createContentType(
    input: {
      name: "Article"
      fields: [
        { name: "title", type: TEXT, nullable: false }
        { name: "published_at", type: TIMESTAMP, nullable: true }
      ]
    }
  ) {
    id
    name
    tableName
    fields {
      name
      type
      nullable
    }
  }
}
```

Schema Types

```graphql
enum FieldType {
  TEXT
  INTEGER
  BOOLEAN
  TIMESTAMP
}

input FieldInput {
  name: String!
  type: FieldType!
  nullable: Boolean!
}

input CreateContentTypeInput {
  name: String!
  fields: [FieldInput!]!
}

type Field {
  name: String!
  type: FieldType!
  nullable: Boolean!
}

type ContentType {
  id: UUID!
  name: String!
  tableName: String!
  fields: [Field!]!
}

type Mutation {
  createContentType(input: CreateContentTypeInput!): ContentType!
}
```

### ➕ 2. addFieldToContentType

Adds a new field (column) to an existing content type.

```graphql
mutation {
  addFieldToContentType(
    contentTypeId: "ARTICLE_UUID_HERE"
    input: { name: "author", type: TEXT, nullable: true }
  ) {
    name
    type
    nullable
  }
}
```

Schema Types

```graphql
input AddFieldInput {
  name: String!
  type: FieldType!
  nullable: Boolean!
}

extend type Mutation {
  addFieldToContentType(contentTypeId: UUID!, input: AddFieldInput!): Field!
}
```

### 📝 3. createContentItem

Inserts a content item (row) into a specific content type table. Since content types are dynamic, this is a generic mutation (or could be scaffolded per type).

Here’s an example for a content type called `Article`:

```graphql
mutation {
  createArticle(
    data: { title: "My first article", published_at: "2025-06-27T09:00:00Z" }
  ) {
    id
    title
    published_at
  }
}
```

> This assumes your GraphQL schema auto-generates mutations per content type like createArticle, updateArticle, etc.

Generated Input Type

```graphql
input ArticleInput {
  title: String!
  published_at: DateTime
}
```

### 🔍 4. queryArticles

```graphql
query {
  articles {
    id
    title
    published_at
  }
}
```

This queries all rows from the article table using the auto-generated Article type.

### 🧩 Field Type System: Enum-Based Approach

- The type of each content field is represented by a strongly-typed enum `PostgresFieldType` in the Domain layer. This enum is the single source of truth for all supported field types (e.g., Text, Integer, Boolean, Timestamp, Decimal).
- The enum is used throughout all layers (API, Application, Infrastructure) for type safety, validation, and maintainability.
- The API exposes the enum as a GraphQL enum, ensuring clients can only submit valid types.
- The Infrastructure layer maps the enum values to actual PostgreSQL column types when creating tables. This mapping is centralized and no longer relies on string-based logic.

#### Example: PostgresFieldType Enum

```csharp
public enum PostgresFieldType
{
    Text,
    Integer,
    Boolean,
    Timestamp,
    Decimal
    // Add more as needed
}
```

#### Example: Enum to SQL Type Mapping (Infrastructure)

```csharp
private static string MapToPostgresType(PostgresFieldType type)
{
    return type switch
    {
        PostgresFieldType.Text => "TEXT",
        PostgresFieldType.Integer => "INTEGER",
        PostgresFieldType.Boolean => "BOOLEAN",
        PostgresFieldType.Timestamp => "TIMESTAMPTZ",
        PostgresFieldType.Decimal => "DECIMAL",
        _ => "TEXT"
    };
}
```

- This approach ensures that adding new field types is a single-responsibility change: update the enum and the mapping logic.
- All validation and mapping for field types is now type-safe and consistent across the codebase.

### 📝 Summary

| Action              | GraphQL Operation       | Comment                        |
| ------------------- | ----------------------- | ------------------------------ |
| Create Content Type | `createContentType`     | Defines schema + creates table |
| Add Field           | `addFieldToContentType` | Alters table structure         |
| Create Content Item | `createXxx`             | Insert into type table         |
| Query Content Items | `xxxPlural`             | Fetch rows from table          |
