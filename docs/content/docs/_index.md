---
title: "Home"
description: "RiverBooks documentation - learn about the architecture and how to get started"
summary: "Documentation for RiverBooks - A Modular Monolith Sample Application"
date: 2024-12-04
weight: 1
---

Welcome to the RiverBooks documentation! This guide will help you understand the architecture of the application and get you up and running quickly.

## What is RiverBooks?

RiverBooks is a sample application demonstrating how to build a **Modular Monolith** using .NET. It serves as a companion project for the [Modular Monolith Courses on Dometrain](https://dometrain.com/author/steve-ardalis-smith/).

Use code **ARDALIS** for 20% off individual courses!

## Sections

- **[Getting Started](/docs/getting-started/)** - Set up your development environment and run the application
- **[Architecture](/docs/architecture/)** - Explore the modular architecture and design patterns

## Key Features

- **Modular Monolith Architecture** - Loosely coupled modules with clear boundaries
- **Domain-Driven Design** - Bounded contexts, aggregates, and domain events
- **Clean Architecture** - Separation of concerns with dependency inversion
- **CQRS** - Command Query Responsibility Segregation using Mediator
- **Integration Events** - Event-driven communication between modules
- **Entity Framework Core** - Database access with migrations
- **MongoDB** - For email outbox and async processing

## Modules

The application is organized into the following modules:

| Module | Description |
|--------|-------------|
| **RiverBooks.Books** | Book catalog management |
| **RiverBooks.Users** | User authentication and profiles |
| **RiverBooks.OrderProcessing** | Order management and checkout |
| **RiverBooks.EmailSending** | Email notifications via outbox pattern |
| **RiverBooks.Reporting** | Sales reports and analytics |
| **RiverBooks.SharedKernel** | Common building blocks |

## Technology Stack

- **.NET 11**
- **ASP.NET Core** - Web API
- **Entity Framework Core** - SQL Server
- **MongoDB** - Document storage
- **Mediator.SourceGen** - CQRS implementation
- **Shouldly** - Test assertions
