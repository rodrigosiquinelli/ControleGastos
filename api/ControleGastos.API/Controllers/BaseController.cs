using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.API.Controllers
{
    // Define a base para todos os controllers, centralizando a rota e o comportamento de API
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
    }
}
