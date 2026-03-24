using System.Net;
using System.Text.Json;

namespace ControleGastos.API.Middlewares;

///Middleware para tratamento exceções.
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Tenta processar a próxima etapa do pipeline da requisição HTTP
            await _next(context);
        }
        catch (Exception ex)
        {
            // Captura qualquer erro não tratado, registra no log e gera uma resposta
            _logger.LogError(ex, "Exceção sem tratamento");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Define o tipo de retorno como JSON e o status code fixo como 500 (Erro Interno)
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Estrutura o objeto de erro para que o cliente da API saiba o que aconteceu
        var response = new
        {
            context.Response.StatusCode,
            Message = "Erro interno no servidor.",
            Detailed = exception.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}