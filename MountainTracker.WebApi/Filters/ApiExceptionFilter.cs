using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MountainTracker.Core.Common.Exceptions;

namespace MountainTracker.WebApi.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Пример: проверяем тип исключения
            if (context.Exception is NotFoundException notFoundEx)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    Error = notFoundEx.Message
                });
                context.ExceptionHandled = true;
            }
            else if (context.Exception is ValidationException validationEx)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Error = validationEx.Message
                });
                context.ExceptionHandled = true;
            }
            else
            {
                // Общий случай
                context.Result = new ObjectResult(new
                {
                    Error = context.Exception.Message
                })
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
