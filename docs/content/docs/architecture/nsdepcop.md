---
title: "Dependency Rules (NsDepCop)"
description: "How RiverBooks enforces namespace dependency rules at compile time with NsDepCop"
summary: "What NsDepCop is, which rules each project enforces, and why the rules live with each module"
date: 2026-10-07
weight: 4
---

RiverBooks uses [NsDepCop](https://github.com/realvizu/NsDepCop) to keep code inside each module from depending on the wrong things. A violation is a **compile error**, so it gets caught the moment someone writes it, not later in code review.

## What NsDepCop Is

NsDepCop is a Roslyn analyzer delivered as a NuGet package. While the compiler builds a project, NsDepCop checks every type reference in that project's source code against a set of rules about which **namespaces** may depend on which other namespaces. The rules live in a `config.nsdepcop` XML file next to the `.csproj`.

A rule set usually starts by allowing everything and then forbids specific dependencies:

```xml
<NsDepCopConfig>
    <Allowed From="*" To="*" />

    <!-- UseCases should not depend on Infrastructure -->
    <Disallowed From="RiverBooks.OrderProcessing.UseCases.*"
                To="RiverBooks.OrderProcessing.Infrastructure.*" />

    <AnalyzerConfig>
        <IssueKind Name="IllegalDependency" Severity="Error" />
    </AnalyzerConfig>
</NsDepCopConfig>
```

`Name.*` matches that namespace and every namespace under it. When a rule is broken, the build fails with a message like this:

```text
error NSDEPCOP01: Illegal namespace reference:
  RiverBooks.OrderProcessing.UseCases.Orders.ListForUser->RiverBooks.OrderProcessing.Infrastructure.Data
```

Because the rules match on namespaces, **a type's namespace has to match its folder**. A type declared in the wrong namespace isn't covered by the rules that apply to its folder.

## Where NsDepCop Fits

RiverBooks enforces module boundaries at three levels, each with a different job:

| Level | Mechanism | What it prevents |
|---|---|---|
| Between modules | Project references: modules reference only other modules' `*.Contracts` projects | Module A using module B's internal types. This won't compile because the reference doesn't exist. |
| Inside a module | NsDepCop `config.nsdepcop` per module | Layering violations, such as UseCases or Endpoints reaching into Infrastructure/Data |
| Inside a module | ArchUnitNET tests (e.g. `RiverBooks.OrderProcessingTests/Arch`) | The same kind of layering rules, written as tests instead of build errors |

Project references already stop one module from depending on another's implementation, so NsDepCop is used for the rules project references can't express: dependencies **within** a project.

## How It's Wired Up

`src/Directory.Build.props` adds NsDepCop to any project that has a `config.nsdepcop` file:

```xml
<PropertyGroup Condition="Exists('$(MSBuildProjectDirectory)\config.nsdepcop')">
  <WarningsAsErrors>$(WarningsAsErrors);NSDEPCOP01</WarningsAsErrors>
</PropertyGroup>
<ItemGroup Condition="Exists('$(MSBuildProjectDirectory)\config.nsdepcop')">
  <PackageReference Include="NsDepCop" Version="3.2.0">...</PackageReference>
</ItemGroup>
```

So:

- **To start enforcing rules in a project**, add a `config.nsdepcop` file next to its `.csproj`. You don't need to edit the `.csproj`.
- The package version is set in one place.
- Projects with no config file (SharedKernel, Contracts, tests, AppHost) don't load the analyzer at all.

## Rules in This Solution

| Project | Rules |
|---|---|
| `RiverBooks.Users` | Domain, UseCases, Interfaces and Integrations may not depend on `Data` |
| `RiverBooks.OrderProcessing` | Domain may not depend on any other layer (Infrastructure, Interfaces, UseCases, Integrations, Endpoints). UseCases, Interfaces, Integrations and Endpoints may not depend on `Infrastructure`. UseCases may not depend on `Endpoints`. |
| `RiverBooks.Books` | `BookEndpoints` and `Integrations` may not depend on `Data`; `Data` may not depend on them |
| `RiverBooks.EmailSending` | `Integrations` (queueing) and `ListEmailsEndpoint` may not depend on the `SendQueuedEmail` pipeline, and the pipeline may not depend on them |
| `RiverBooks.Reporting` | `ReportEndpoints` (querying) and `Integrations` (ingesting orders) may not depend on each other |
| `RiverBooks.Web` | Host guard for OrderProcessing (see below) |

The modules aren't all organized the same way. Users and OrderProcessing use full Domain/UseCases/Infrastructure layers, while Books, EmailSending and Reporting are flatter. Each config is written for its own module's folder structure.

### The Web Host Guard

Most of OrderProcessing's types are `internal`; its only public type is `OrderProcessingModuleServicesExtensions`. There's one complication. RiverBooks uses the [Mediator](https://github.com/martinothamar/Mediator) **source generator**, which runs in `RiverBooks.Web` and writes code that refers to every handler type directly. For that generated code to compile, OrderProcessing must declare:

```csharp
[assembly: InternalsVisibleTo("RiverBooks.Web")]
```

That makes all of OrderProcessing's internal types visible to the host's hand-written code too. `RiverBooks.Web/config.nsdepcop` restores the boundary:

- `VisibleMembers` limits the `RiverBooks.OrderProcessing` root namespace to `OrderProcessingModuleServicesExtensions`.
- `Disallowed` rules block the module's internal namespaces (`Domain`, `Endpoints`, `Infrastructure`, `Integrations`, `Interfaces`, `UseCases`, `Data`).
- `RiverBooks.OrderProcessing.Contracts` stays available.

NsDepCop skips generated code, so `Mediator.g.cs` isn't affected.

**When you make another module's handlers internal**, give it the same `InternalsVisibleTo("RiverBooks.Web")` and add a matching block to the Web config. A module whose types are public doesn't need a host guard, because the host can only see what's already public.

## One Global Config vs. One Per Project

NsDepCop supports both a single shared config and per-project configs, which can be layered with the `InheritanceDepth` attribute. RiverBooks uses **one per project, with the shared setup in `Directory.Build.props`**. These are the trade-offs:

| | One global config | One config per project |
|---|---|---|
| **Where rules live** | One file holds every module's rules, and every project reads all of them | Each rule set sits in its own module and moves with it if the module is extracted |
| **Wildcards** | `*` only works at the end of a pattern, so you can't write one generic rule like "any module's Domain must not depend on any module's Infrastructure". Every module's rules still have to be written out. | Same limit, but each file only covers its own module |
| **Different layouts** | Has to handle Users' `Data` alongside OrderProcessing's `Infrastructure` and the flatter modules | Each config matches its own module's folders |
| **Drift** | Can't drift: there's only one place to look | Configs can drift, and a module can end up with no rules at all |
| **Setup** | The NsDepCop package only reads `config.nsdepcop` from the project folder, so a root file needs `InheritanceDepth` or extra MSBuild wiring | Works as shipped |
| **Merge conflicts** | Every team edits the same file | Changes stay inside the module |

A global file would mostly be useful for rules between modules, and project references already enforce those. Per-project configs therefore fit a modular monolith better. Putting the package reference in `Directory.Build.props` limits drift: adding a config file is all it takes to opt a project in, and the version is the same everywhere.

If rules that genuinely apply to every module show up later, put them in a parent `config.nsdepcop` and have module configs inherit it with `InheritanceDepth`, rather than copying them into each file.

## Adding or Changing Rules

1. Edit (or create) the project's `config.nsdepcop`.
2. Build. Any existing violations show up as `NSDEPCOP01` errors.
3. To confirm a new rule actually triggers, temporarily add a reference that breaks it, build, and check that the build fails. Then remove the reference.

See the [NsDepCop documentation](https://github.com/realvizu/NsDepCop/blob/master/doc/Help.md) for the full config reference, including `VisibleMembers`, `ChildCanDependOnParentImplicitly`, assembly-level rules and `ExcludedFiles`.
