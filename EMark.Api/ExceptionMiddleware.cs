using EMark.Application.Exeptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace EMark.Api
{
    public class ExceptionMiddleware : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ConfigureExceptionHandler(context);
            context.ExceptionHandled = true;
        }
        private static void ConfigureExceptionHandler(ExceptionContext context)
        {
            Exception exception = context.Exception;

            switch (exception)
            {
                case NotFoundException:
                    SetExceptionResult(context, exception, HttpStatusCode.NotFound);
                    break;
                case ValidationException:
                    SetExceptionResult(context, exception, HttpStatusCode.BadRequest);
                    break;
                case UnauthorizedAccessException:
                    SetExceptionResult(context, exception, HttpStatusCode.Forbidden);
                    break; ;
                default:
                    SetExceptionResult(context, exception, HttpStatusCode.InternalServerError);
                    break;
            }
        }

        private static void SetExceptionResult(
            ExceptionContext context,
            Exception exception,
            HttpStatusCode statusCode)
        {
            context.Result = new JsonResult(exception.Message)
            {
                StatusCode = (int)statusCode
            };
        }
    }
}
