---
title: "Dependencies Diagram"
description: "Interactive dependency diagram showing module relationships"
summary: "Visual overview of how RiverBooks modules depend on each other"
date: 2024-12-04
weight: 2
---

This interactive diagram shows the dependencies between all projects in the RiverBooks solution. Modules are color-coded; non-module projects are grey. Note that main module projects (those without `.Tests` or `.Contracts` suffixes) should only be referenced by their own tests project and the application host (`RiverBooks.Web`).

## Interactive Dependency Diagram

Built with [nmbl](https://nmbl.dev).

<p><a href="/diagrams/riverbooks.dynamic.html" target="_blank" rel="noopener">Open in new tab</a></p>

<iframe 
  src="/diagrams/riverbooks.dynamic.html" 
  width="100%" 
  height="800px" 
  style="border: 1px solid #ccc; border-radius: 8px;"
  title="RiverBooks Dependency Diagram">
</iframe>

## Understanding the Diagram

The diagram visualizes:

- **Nodes**: Each project in the solution
- **Edges**: Dependencies between projects (arrows point from dependent to dependency)
- **Colors**: Different colors may represent different module types

## Key Observations

### Module Independence

Notice how each feature module (Books, Users, OrderProcessing, etc.) has its own Contracts project. This allows other modules to depend on the interface without coupling to the implementation. Dependencies *within* each module are enforced at compile time by NsDepCop; see [Dependency Rules (NsDepCop)]({{< relref "nsdepcop" >}}).

### SharedKernel

The `RiverBooks.SharedKernel` project is referenced by multiple modules. It contains:
- Base classes for entities and value objects
- Common interfaces (like `IDomainEvent`)
- Cross-cutting concerns

### Web Host

The `RiverBooks.Web` project references all modules to compose them into a single running application.

## Static Version

If the interactive diagram doesn't load, you can [view the HTML file directly](/diagrams/riverbooks.dynamic.html).
