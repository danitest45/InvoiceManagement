using InvoiceManagement.Api.Models;
using InvoiceManagement.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace InvoiceManagement.Api.Middlewares
{
    /// <summary>
    /// Middleware responsible for handling global exceptions
    /// and formatting error responses.
    /// </summary>
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Intercepts the HTTP request and handles exceptions globally.
        /// </summary>
        /// <param name="context">Current HTTP context.</param>
        /// <returns>Task representing the request pipeline execution.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                await HandleBusinessException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleGenericException(context, ex);
            }
        }

        private static async Task HandleBusinessException(
            HttpContext context,
            BusinessException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new ErrorResponse
            {
                Message = ex.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        private static async Task HandleGenericException(
            HttpContext context,
            Exception ex)
        {
            ArgumentNullException.ThrowIfNull(ex);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ErrorResponse
            {
                Message = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
