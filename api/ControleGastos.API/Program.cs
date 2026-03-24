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
    // Cria um escopo temporário para garantir a criação do banco e popular dados iniciais
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ControleGastosDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        dbContext.Database.EnsureCreated();
        await SeedData.InitializeAsync(dbContext);
    }
}

// Configura o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleGastos.Api v1");
        c.DefaultModelsExpandDepth(-1); //Oculta a seção de Schemas
    });
    // Redireciona a rota raiz (/) diretamente para o Swagger para facilitar
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

// Registra o middleware customizado para capturar e tratar erros globais da aplicação
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.MapControllers();


app.Run();
