using System;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Storage.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace WatchCollection.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Only log unexpected exceptions - expected business logic exceptions are noise
            var isExpectedException = ex is (
                EntityNotFoundException or BidTooLowException 
                or DomainInvalidOperationException or EntityUnavailableException 
                or InvalidAdvertisementStatusException or MappingException 
                or FileValidationException or DbUpdateException);

            if (!isExpectedException)
            {
                _logger.LogError(ex, "Unexpected exception handled by middleware");
            }
           
            var (statusCode, title, detail) = ex switch
            {
                // Custom exceptions with specific handling
                BidTooLowException btl =>
                    (StatusCodes.Status400BadRequest, "Bid too low", btl.Message),

                EntityNotFoundException enf =>
                    (StatusCodes.Status404NotFound, 
                     "Entity not found", 
                     $"Entity with id '{enf.EntityId}' not found."),

                DomainInvalidOperationException ioe =>
                    (StatusCodes.Status400BadRequest, "Invalid operation", ioe.Message),

                EntityUnavailableException eue =>
                    (StatusCodes.Status409Conflict, "Entity unavailable", eue.Message),

                InvalidAdvertisementStatusException ias =>
                    (StatusCodes.Status400BadRequest, "Invalid advertisement status", ias.Message),

                MappingException me =>
                    (StatusCodes.Status500InternalServerError, "Mapping error", me.Message),

                FileValidationException fve =>
                    (StatusCodes.Status400BadRequest, "File validation error", fve.Message),

                DbUpdateException due =>
                    (StatusCodes.Status409Conflict, 
                     "Data conflict", 
                     due.InnerException?.Message ?? "Unable to save changes due to data integrity constraints."),

                // Unhandled exceptions
                _ =>
                    (StatusCodes.Status500InternalServerError,
                    "Unexpected error",
                    "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var Problem = new
            {
                type = "https://httpstatuses.io/" + statusCode,
                title,
                detail
            };

            await context.Response.WriteAsJsonAsync(Problem);
        }
    }

}
