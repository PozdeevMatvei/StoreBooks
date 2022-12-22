using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Store.Web.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Web.App.Middlewares
{
    public class ExceptionHendlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHendlingMiddleware> _logger;

        public ExceptionHendlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHendlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(httpContext,
                    HttpStatusCode.InternalServerError,
                    ex.Message,
                    "Internal server error");
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string exceptionMessage,
            string message)
        {
            _logger.LogError(exceptionMessage);

            HttpResponse response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            ErrorModel errorModel = new()
            {
                Message = message,
                StatusCode = (int)statusCode
            };

            string result = JsonSerializer.Serialize(errorModel);

            await response.WriteAsync(result);
        }


    }
}
