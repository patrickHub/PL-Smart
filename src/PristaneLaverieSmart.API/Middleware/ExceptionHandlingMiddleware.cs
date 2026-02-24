namespace PristaneLaverieSmart.API.Middleware;

using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using PristaneLaverieSmart.API.Contracts;
using PristaneLaverieSmart.Application.Common.Exceptions;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                );

            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    Title: "Validation failed",
                    Status: StatusCodes.Status400BadRequest,
                    Detail: ex.Message,
                    Errors: new Dictionary<string, string[]>(errors),
                    TraceId: context.TraceIdentifier
                )
            );
        }
        catch (ValidationException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    Title: "Validation failed",
                    Status: StatusCodes.Status400BadRequest,
                    Detail: ex.Message,
                    Errors: new Dictionary<string, string[]>(ex.Errors),
                    TraceId: context.TraceIdentifier
                ));
        }
        catch(NotFoundException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.NotFound,
                new ApiErrorResponse(
                    Title: "Resource not found",
                    Status: StatusCodes.Status404NotFound,
                    Detail: ex.Message,
                    TraceId: context.TraceIdentifier
                )
            );
        }
        catch(ArgumentException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    Title: "Bad Request",
                    Status: StatusCodes.Status400BadRequest,
                    Detail: ex.Message,
                    TraceId: context.TraceIdentifier
                )
            );
        }
        catch (BadHttpRequestException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    Title: "Invalid request",
                    Status: StatusCodes.Status400BadRequest,
                    Detail: ex.Message,
                    TraceId: context.TraceIdentifier
                ));
        }
        catch (JsonException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    Title: "Invalid JSON payload",
                    Status: StatusCodes.Status400BadRequest,
                    Detail: ex.Message,
                    TraceId: context.TraceIdentifier
                ));
        }
        catch(Exception ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.InternalServerError,
                new ApiErrorResponse(
                    Title: "Internal Server Error",
                    Status: StatusCodes.Status500InternalServerError,
                    Detail: ex.Message,
                    TraceId: context.TraceIdentifier
                )
            );
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode code, ApiErrorResponse apiError)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "Application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(apiError, JsonOptions));
    }
}