using Microsoft.AspNetCore.Mvc;

namespace ProductControllCrohmal.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult HandleException(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => NotFound(new { message = ex.Message }),
                InvalidOperationException => BadRequest(new { message = ex.Message }),
                UnauthorizedAccessException => Unauthorized(new { message = ex.Message }),
                ArgumentException => BadRequest(new { message = ex.Message }),
                NotImplementedException => StatusCode(501, new { message = ex.Message }),
                _ => StatusCode(500, new { message = "Внутренняя ошибка сервера.", detail = ex.Message })
            };
        }
    }
}
