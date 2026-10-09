---
title: "Overview"
description: "Explore the modular monolith architecture of RiverBooks"
summary: "Learn about the architecture patterns and design decisions"
date: 2024-12-04
weight: 1
---

RiverBooks demonstrates a [Modular Monolith architecture](https://modularmonoliths.com/) - a middle ground between traditional monoliths and microservices that offers the best of both worlds.

## What is a Modular Monolith?

A modular monolith is a single deployable application that is internally organized into loosely coupled modules. Each module:

- Has its own [bounded context](https://deviq.com/domain-driven-design/bounded-context)
- Owns its data and database schema
- Communicates with other modules through well-defined interfaces/contracts
- Can be developed and tested independently

## Benefits

- **Simpler Operations** - Single deployment, no distributed system complexity
- **Clear Boundaries** - Modules enforce separation of concerns
- **Easy Evolution** - Modules can be extracted to services later

## Tradeoffs

- **Single Language/Framework** - All modules share the same programming language and framework, generally (different .NET languages could be used)
- **No Per-Module Scaling** -- Unlike microservices, if more resources are needed the whole monolith must be scaled (up and/or out)

## Project Structure

```
src/
├── RiverBooks.Web/              # Host application (API endpoints)
├── RiverBooks.Books/            # Books catalog module
├── RiverBooks.Books.Contracts/  # Public DTOs and interfaces
├── RiverBooks.Users/            # User management module
├── RiverBooks.Users.Contracts/
├── RiverBooks.OrderProcessing/  # Order management module
├── RiverBooks.OrderProcessing.Contracts/
├── RiverBooks.EmailSending/     # Email notification module
├── RiverBooks.EmailSending.Contracts/
├── RiverBooks.Reporting/        # Reporting module
└── RiverBooks.SharedKernel/     # Common building blocks
```

## Module Communication

Modules communicate through two primary mechanisms:

### 1. Contracts

Each module exposes a `Contracts` project containing:
- DTOs (Data Transfer Objects)
- Query/Command definitions
- Integration event definitions

Other modules reference only the contracts, never the implementation.

### 2. Integration Events

Modules publish domain events that other modules can subscribe to:

```csharp
// Published by OrderProcessing module
public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid UserId,
    DateTimeOffset OrderDate
) : IIntegrationEvent;
```

## Key Patterns

| Pattern | Usage |
|---------|-------|
| **[CQRS](https://deviq.com/design-patterns/cqrs-pattern)** | Separate commands (writes) from queries (reads) |
| **[Mediator](https://deviq.com/design-patterns/mediator-pattern)** | Decouple request handlers from controllers |
| **[Repository](https://deviq.com/design-patterns/repository-pattern)** | Abstract data access behind interfaces |
| **[Outbox](https://deviq.com/design-patterns/outbox-pattern)** | Reliable event publishing (EmailSending module) |
| **[Domain Events](https://deviq.com/design-patterns/domain-events-pattern)** | Notify interested parties of state changes |

## Explore More

- [Dependencies Diagram]({{< relref "dependencies" >}}) - Visual overview of module dependencies
- [Module Diagrams](/docs/architecture/module-diagrams/) - Detailed module structure diagrams
