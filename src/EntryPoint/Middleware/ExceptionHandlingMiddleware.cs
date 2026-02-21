using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Domain.Common;

namespace EntryPoint.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException nf)
            {
                _logger.LogInformation(nf, "Not found: {Message}", nf.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Resource not found",
                    Status = context.Response.StatusCode,
                    Detail = nf.Message,
                    Instance = context.Request.Path
                };
                problem.Extensions["exceptionType"] = nf.GetType().Name;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
            catch (DuplicateException dup)
            {
                _logger.LogWarning(dup, "Duplicate: {Message}", dup.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Resource conflict",
                    Status = context.Response.StatusCode,
                    Detail = dup.Message,
                    Instance = context.Request.Path
                };
                problem.Extensions["exceptionType"] = dup.GetType().Name;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
            catch (BusinessRuleException br)
            {
                _logger.LogWarning(br, "Business rule violation: {Message}", br.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Business rule violation",
                    Status = context.Response.StatusCode,
                    Detail = br.Message,
                    Instance = context.Request.Path
                };
                problem.Extensions["exceptionType"] = br.GetType().Name;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
            catch (DomainException dex)
            {
                _logger.LogWarning(dex, "Domain error: {Message}", dex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Domain error",
                    Status = context.Response.StatusCode,
                    Detail = dex.Message,
                    Instance = context.Request.Path
                };
                problem.Extensions["exceptionType"] = dex.GetType().Name;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = context.Response.StatusCode,
                    Detail = _env.IsDevelopment() ? ex.ToString() : "Please contact support.",
                    Instance = context.Request.Path
                };
                problem.Extensions["exceptionType"] = ex.GetType().Name;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
        }
    }
}
