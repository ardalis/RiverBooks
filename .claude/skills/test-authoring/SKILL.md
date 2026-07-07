---
name: test-authoring
description: Conventions for writing xUnit tests. Covers naming, assertion style (Shouldly), mocking (NSubstitute), fixture patterns, and the rules that make tests meaningful vs. ornamental. Load this skill before writing or reviewing any test file.
invocable: true
---

# Test Authoring Conventions

## Stack

| Concern | Library |
|---|---|
| Framework | xUnit |
| Assertions | Shouldly |
| Mocking | NSubstitute |
| Blazor components | bunit |
| API integration | `Microsoft.AspNetCore.Mvc.Testing` |
| Coverage | coverlet.collector |

---

## Naming

### Test classes

Pattern: `<Subject>_<Scenario>`

The scenario captures what is true about the inputs or context, not the outcome.

```
InMemoryAnnotationRepository_GetAll
CreateAnnotationEndpoint_Returns201
CreateAnnotationEndpoint_Returns409
AnnotationLayer_RendersHostElement
```

When the same operation produces multiple distinct outcomes, create **separate classes** — one per outcome — rather than crowding them all into one class.

### Test methods

Pattern: `<ExpectedOutcome>_<Condition>`

```csharp
ReturnsEmpty_WhenImageHasNoAnnotations()
Returns201_WithLocationHeader()
ContainsHostDiv_GivenDefaultParameters()
FiresOnChange_WhenValueChanges()
```

The method name must be self-documenting. A reader should understand what the test proves without reading the body.

---

## Structure

### AAA inline — no setup methods

All arrange/act/assert lives inside the test method body. Do not use constructor setup, `IClassFixture` shared state for unit tests, or `[SetUp]`-style attributes.

```csharp
[Fact]
public async Task ReturnsEmpty_WhenImageHasNoAnnotations()
{
    // Arrange
    var repo = new InMemoryAnnotationRepository();

    // Act
    var result = await repo.GetAllAsync("img-1");

    // Assert
    result.ShouldBeEmpty();
}
```

### One behaviour per test

Each test proves exactly one thing. If you need three `ShouldBe` calls, you probably need three tests.

### File layout

One test class per file. File name matches class name: `CreateAnnotationEndpointTests.cs`.

---

## Assertions (Shouldly)

Use Shouldly fluent assertions — never bare `Assert.*` calls.

```csharp
// Preferred
result.ShouldBeEmpty();
response.StatusCode.ShouldBe(HttpStatusCode.Created);
annotation.Label.ShouldBe("Tumour");
result.ShouldNotBeNull();
fired.ShouldBeTrue();
items.Count.ShouldBe(3);

// Never
Assert.Empty(result);
Assert.Equal(HttpStatusCode.Created, response.StatusCode);
```

---

## Mocking (NSubstitute)

```csharp
var store = Substitute.For<IAnnotationStore>();
store.GetAsync(Arg.Any<string>()).Returns(Task.FromResult(annotations));

// Verification
await store.Received(1).SaveAsync(Arg.Is<Annotation>(a => a.Label == "Tumour"));
```

Do not use Moq. Do not hand-roll fakes when NSubstitute can substitute the interface directly.

---

## Fixtures (test data builders)

For anything more complex than `new Foo()`, use a `file static class` fixture with a `Make()` factory:

```csharp
file static class AnnotationFixtures
{
    public static Annotation Make(string id = "a1", string label = "Test") =>
        new Annotation { Id = id, Label = label, ImageId = "img-1" };
}
```

- Scope to `file` so it's invisible outside the test file
- Provide sensible defaults so tests only specify what they actually care about
- Keep factory data minimal — don't populate fields the test ignores

---

## Integration tests (API layer)

Use a custom `WebApplicationFactory<Program>` subclass:

```csharp
public class AnnotationsApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Replace real services with in-memory doubles
            services.AddSingleton<IAnnotationRepository, InMemoryAnnotationRepository>();
        });
    }
}
```

Inherit `IClassFixture<AnnotationsApiFactory>` on the test class and inject `HttpClient` through the constructor.

---

## Blazor component tests (bunit)

Inherit from `BunitContext`. Register services in the constructor:

```csharp
public class AnnotationLayer_RendersHostElement : BunitContext
{
    private readonly IAnnotationsInterop _interop = Substitute.For<IAnnotationsInterop>();

    public AnnotationLayer_RendersHostElement()
    {
        Services.AddSingleton(_interop);
    }

    [Fact]
    public void ContainsHostDiv_GivenDefaultParameters()
    {
        var cut = Render<AnnotationLayer>();
        cut.Find("div[id='annotation-host']").ShouldNotBeNull();
    }
}
```

---

## The most important rule

**A test that passes when the code is broken is worse than no test.**

Before considering a test done, ask: _if someone deleted the key line this test is supposed to exercise, would the test fail?_

If the answer is no, the test is asserting an incidental side-effect or a tautology. Rewrite it to assert the direct observable outcome of the behaviour under test.

Good targets for assertions:
- Return values
- State mutations observable through a public API
- Calls received on a substitute (when the side-effect IS the behaviour)
- HTTP status codes and response bodies (integration tests)

Bad targets:
- Internal implementation details (private fields, call order between private methods)
- Framework behaviour (e.g., DI resolves correctly, HttpClient sends a request)
- Things that are always true regardless of whether the code runs

---

## Where to put new test files

> **Adapt this table to match your project's solution structure.**

| Source project | Test project |
|---|---|
| `src/Acme.Store.Api/` | `tests/Acme.Store.Api.Tests/` |
| `src/Acme.Store.Blazor.Shared/` | `tests/Acme.Store.Blazor.Shared.Tests/` |
| `src/Acme.Store.BlazorClient/` | `tests/Acme.Store.BlazorClient.Tests/` |

Mirror the source folder structure inside the test project. If `src/Acme.Store.Api/Endpoints/Validations/` holds the source, the test file goes in `tests/Acme.Store.Api.Tests/Endpoints/Validations/`.
