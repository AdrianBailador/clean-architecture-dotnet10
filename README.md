# Clean Architecture in .NET 10

Companion repo for [Lesson 01 of the Software Architecture Fundamentals course](https://adrianbailador.github.io/courses/01-software-architecture-fundamentals) on [adrianbailador.github.io](https://adrianbailador.github.io).

A small order-placing API built to show the Dependency Rule as working code, not a diagram: source code dependencies only point inward, toward the domain. **The dependency rule is enforced here by a test, not just by convention** — see [`CleanArchitecture.ArchitectureTests`](tests/CleanArchitecture.ArchitectureTests/DependencyRuleTests.cs).

## Structure

```
src/
  CleanArchitecture.Domain          — Order, OrderLine, Money. Zero dependencies, not even on an interface.
  CleanArchitecture.Application     — The PlaceOrder use case and the ports (IOrderRepository) it needs. Depends only on Domain.
  CleanArchitecture.Infrastructure  — EF Core (InMemory provider), implementing the Application ports.
  CleanArchitecture.Api             — ASP.NET Core minimal API. The only project that wires Application and Infrastructure together.
tests/
  CleanArchitecture.Domain.Tests       — Business rules: discounts, status transitions, validation.
  CleanArchitecture.Application.Tests  — The use case, tested against a fake repository — no EF Core involved.
  CleanArchitecture.ArchitectureTests  — Reflection-based assertions that the Dependency Rule actually holds.
```

## The one business rule worth knowing about

Orders of $100 or more get a 10% loyalty discount. It lives in exactly one place: [`Order.Total()`](src/CleanArchitecture.Domain/Order.cs). Nothing in the Application, Infrastructure, or Api layer knows this rule exists — they just call `order.Total()`.

## Running it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet test
dotnet run --project src/CleanArchitecture.Api
```

Then, with the API running:

```bash
curl -X POST http://localhost:5299/orders \
  -H "Content-Type: application/json" \
  -d '{"customerEmail":"you@example.com","lines":[{"productName":"Monitor","quantity":1,"unitPrice":150}]}'
```

The response total reflects the 10% discount (`135`, not `150`) — the same rule that `CleanArchitecture.Domain.Tests` verifies in isolation.

## What to actually look at

- [`Order.cs`](src/CleanArchitecture.Domain/Order.cs) — every rule about what makes an order valid, in one place.
- [`IOrderRepository.cs`](src/CleanArchitecture.Application/Ports/IOrderRepository.cs) — the port. Application defines it, Infrastructure implements it.
- [`OrderConfiguration.cs`](src/CleanArchitecture.Infrastructure/Persistence/OrderConfiguration.cs) — where EF Core's owned-entity mapping, constructor binding, and backing fields meet a domain model that was never designed with an ORM in mind.
- [`DependencyRuleTests.cs`](tests/CleanArchitecture.ArchitectureTests/DependencyRuleTests.cs) — the test suite that fails the build if someone breaks the rule this whole sample is about.

## License

MIT — see [LICENSE](LICENSE).
