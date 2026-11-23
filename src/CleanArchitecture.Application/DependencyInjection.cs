using CleanArchitecture.Application.UseCases.PlaceOrder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services.AddScoped<IPlaceOrderUseCase, PlaceOrderInteractor>();
}
