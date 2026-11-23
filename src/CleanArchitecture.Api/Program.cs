using CleanArchitecture.Application;
using CleanArchitecture.Application.Ports;
using CleanArchitecture.Application.UseCases.PlaceOrder;
using CleanArchitecture.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// This is the only place in the solution where Application and
// Infrastructure are wired together. Everything else only sees interfaces.
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (PlaceOrderRequest request, IPlaceOrderUseCase useCase, CancellationToken ct) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return Results.Created($"/orders/{result.OrderId}", result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(new { error = ex.Message });
    }
})
.WithName("PlaceOrder");

app.MapGet("/orders/{id:guid}", async (Guid id, IOrderRepository orders, CancellationToken ct) =>
{
    var order = await orders.GetAsync(id, ct);
    return order is null
        ? Results.NotFound()
        : Results.Ok(new
        {
            order.Id,
            order.CustomerEmail,
            Status = order.Status.ToString(),
            Total = order.Total().Amount,
            order.Total().Currency,
            Lines = order.Lines.Select(l => new { l.ProductName, l.Quantity, UnitPrice = l.UnitPrice.Amount }),
        });
})
.WithName("GetOrder");

app.Run();
