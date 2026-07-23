# Routine – Copilot Instructions

## Build, test, and lint

```bash
dotnet build          # build everything
dotnet test           # run full test suite
```

Run a single test class or method:

```bash
dotnet test --filter "FullyQualifiedName~ObjectServiceTest_PerformOperation"
dotnet test --filter "Name=ShouldReturnApplicationModel"
```

All projects target **net10.0**, use **C# 13**, and have `TreatWarningsAsErrors=true`. No warnings are allowed.

## Architecture overview

Routine exposes arbitrary C# classes as an HTTP API by mapping the application's "coding style" (which types, methods, and properties to expose) onto a uniform service contract. The pipeline has four layers:

```
Core       – abstract service contract (IObjectService, ApplicationModel, ObjectModel, …)
Engine     – implements IObjectService using a ConventionBasedCodingStyle; the domain model
             (DomainType, DomainObject, DomainOperation) bridges CodingStyle ↔ Core models
Interception – wraps IObjectService in an interceptor chain (InterceptedObjectService)
Service    – ASP.NET Core middleware (RoutineMiddleware) handles HTTP ↔ IObjectService
Client     – Rapplication / Robject / Roperation for consuming a remote Routine service
```

**Entry point for hosts:** `app.UseRoutine(codingStyle: cs => cs.FromBasic()...)` in `Program.cs`.  
**Entry point for clients:** `BuildRoutine.Context().AsServiceClient(...)`.  
All builder objects originate from the static `BuildRoutine` factory class.

## Convention system

`ConventionBasedCodingStyle` (and its siblings for interception, service, and client configurations) holds typed `ConventionBasedConfiguration<TFrom, TResult>` slots (e.g. `Locator`, `IdExtractor`, `Datas`, `Operations`).

- Conventions are evaluated in **most-specific-first** order, stopping at the first match.
- Specificity is controlled by **layers**: `NextLayer()` bumps the current layer; `Override(...)` forces `MostSpecific`. Conventions added on a higher layer win regardless of declaration order.
- Every convention supports `.When(predicate)` to restrict when it applies.
- **Patterns** (`PatternBuilder<T>`) bundle multiple convention assignments into reusable units and are merged via `.Use(cs => cs.SomePattern())`.

```csharp
// Built-in patterns (CodingStylePatterns.cs, ServicePatterns.cs)
cs.Use(p => p.ParseableValueTypePattern())
cs.Use(p => p.EnumPattern())
cs.Use(p => p.AutoMarkWithAttributesPattern())
cs.Use(p => p.VirtualTypePattern())
```

## Key conventions in this codebase

- **`BuildRoutine` is the only public factory.** Never instantiate builders or configurations directly in application or test code.
- **Layered overrides:** When writing tests or extending configurations, call `.NextLayer()` after the base setup so your additions take precedence without breaking existing conventions.
- **`TypeInfo` is a global cache.** Tests must call `TypeInfo.Clear()` in `[SetUp]` (already done in `CoreTestBase.SetUp()`). Always call `base.SetUp()` in test subclasses.
- **Test base classes:** `CoreTestBase` (model/data builders for Core-layer tests) and `ObjectServiceTestBase` (sets up a real `ObjectService` with a `ConventionBasedCodingStyle` scoped to the test namespace). Extend the right base.
- **`InternalsVisibleTo`:** `Routine.Test` and `Routine.Test.Performance` can access internal members; tests may rely on this.
- **Virtual types:** `VirtualType` / `VirtualMethod` / `VirtualParameter` let you inject non-CLR types into the coding style without creating real .NET classes.
- **`ImplicitUsings`:** `System.Collections` is globally imported for all projects. `Moq` and `NUnit.Framework` are globally imported in the test project.
- **Warnings are errors:** Fix all compiler warnings; do not suppress them with pragmas.