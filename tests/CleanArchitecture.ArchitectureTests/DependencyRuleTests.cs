using System.Reflection;

namespace CleanArchitecture.ArchitectureTests;

/// <summary>
/// This is the lesson, enforced by a test instead of a diagram: the
/// Dependency Rule says source code dependencies only point inward. If
/// someone adds a project reference that breaks it, these tests fail on the
/// next build, not six months later when nobody remembers why it matters.
/// </summary>
public class DependencyRuleTests
{
    private static readonly Assembly DomainAssembly = typeof(global::CleanArchitecture.Domain.Order).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(global::CleanArchitecture.Application.UseCases.PlaceOrder.IPlaceOrderUseCase).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(global::CleanArchitecture.Infrastructure.Persistence.CleanArchitectureDbContext).Assembly;

    private static IEnumerable<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(a => a.Name!);

    [Fact]
    public void Domain_does_not_reference_Application()
    {
        Assert.DoesNotContain(ApplicationAssembly.GetName().Name, ReferencedAssemblyNames(DomainAssembly));
    }

    [Fact]
    public void Domain_does_not_reference_Infrastructure()
    {
        Assert.DoesNotContain(InfrastructureAssembly.GetName().Name, ReferencedAssemblyNames(DomainAssembly));
    }

    [Fact]
    public void Domain_does_not_reference_any_Entity_Framework_package()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(DomainAssembly), name => name.StartsWith("Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Application_does_not_reference_Infrastructure()
    {
        Assert.DoesNotContain(InfrastructureAssembly.GetName().Name, ReferencedAssemblyNames(ApplicationAssembly));
    }

    [Fact]
    public void Application_does_not_reference_any_Entity_Framework_package()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(ApplicationAssembly), name => name.StartsWith("Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Application_does_not_reference_AspNetCore()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(ApplicationAssembly), name => name.StartsWith("Microsoft.AspNetCore"));
    }
}
