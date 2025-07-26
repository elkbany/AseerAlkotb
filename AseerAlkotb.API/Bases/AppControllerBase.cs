using AseerAlkotb.Domain.Interfaces.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AseerAlkotb.API.Bases
{
    public class AppControllerBase : ControllerBase
    {
        #region Actions
        public IActionResult ApiResult<T>(IApiResponse<T> response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(response),
                HttpStatusCode.Created => new CreatedResult(string.Empty, response),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                HttpStatusCode.Accepted => new AcceptedResult(string.Empty, response),
                HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                HttpStatusCode.NoContent => new NoContentResult(),
                _ => new BadRequestObjectResult(response)

            };
        }
        #endregion
    }
}
