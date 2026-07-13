using NetArchTest.Rules;
using YouG.Application;
using YouG.Domain;
using YouG.Infrastructure;

namespace YouG.ArchitectureTests;

public class DependencyRuleTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(DomainAssemblyMarker).Assembly;
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;
    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(InfrastructureAssemblyMarker).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Any_Other_Layer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("YouG.Application", "YouG.Infrastructure", "YouG.API",
                "Microsoft.EntityFrameworkCore", "MediatR")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_API()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("YouG.Infrastructure", "YouG.API")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_API()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("YouG.API")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Dependency rule violated by: " + string.Join(", ", result.FailingTypeNames ?? []);
}
