using ControleGastos.API.Helpers;
using ControleGastos.API.Middlewares;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Infrastructure;
using ControleGastos.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Injeção de dependências
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddControleGastosInfrastructure(builder.Configuration);
builder.Services.AddControleGastosServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ControleGastosDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        dbContext.Database.EnsureCreated();
        await SeedData.InitializeAsync(dbContext);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleGastos.Api v1");
        c.DefaultModelsExpandDepth(-1); //Hides the schemas section
    });
    // Redirect root to Swagger UI for convenience
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.MapControllers();


app.Run();
