using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Yaba.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _enviroment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment enviroment, ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._enviroment = enviroment;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Message: {0}", ex.Message);
                await this.HandleBadRequest(ex.Message, ex.StackTrace, context);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                await this.HandleInternalServerError(ex.Message, ex.StackTrace, context);
            }
        }

        #region Priv methods
        private async Task HandleBadRequest(string message, string? stackTracing, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await HandleError(message, stackTracing, context);
        }

        private async Task HandleInternalServerError(string message, string? stackTracing, HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (_enviroment.IsProduction())
                message = "Ocorreu um problema no servidor";

            await HandleError(message, stackTracing, context);
        }

        private async Task HandleError(string message, string? stackTracing, HttpContext context)
        {
            ErrorExceptionResult response;

            if (_enviroment.IsProduction())
            {
                response = new ErrorExceptionResult(false, message);
            }
            else
                response = new ErrorExceptionResult(false, message, stackTracing);

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        #endregion
    }

    internal class ErrorExceptionResult
    {
        public ErrorExceptionResult(bool success, string message, string? details = null)
        {
            this.Success = success;
            this.Message = message;
            this.Details = details;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }
}
