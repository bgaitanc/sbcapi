using System.Net;
using Microsoft.AspNetCore.Mvc;
using SBC.Domain.Exceptions;

namespace SBC.Api.Controllers.Base;

/// <summary>
/// Base controller for all controllers in the application.
/// </summary>
public class SbcControllerBase : ControllerBase
{
    /// <summary>
    /// Executes an asynchronous service action and returns a standardized response.
    /// </summary>
    /// <param name="action">The asynchronous method to execute, represented as a function.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an
    /// <see cref="ActionResult{DataFlowHubGenericResponse}"/> containing the result of the operation
    /// and a corresponding HTTP status code.
    /// </returns>
    protected async Task<ActionResult<SbcGenericResponse>> ExecuteServiceAsync(Func<Task> action)
    {
        try
        {
            await action.Invoke();

            return NoContent();
        }
        catch (SbcException sbcEx)
        {
            return GenerateActionResult(new SbcGenericResponse
            {
                StatusCode = sbcEx.StatusCode,
                Success = false,
                Message = sbcEx.Message
            });
        }
        catch (Exception e)
        {
            return GenerateActionResult(new SbcGenericResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Success = false,
                Message = e.Message
            });
        }
    }

    /// <summary>
    /// Executes the provided asynchronous service action and returns a standardized response.
    /// </summary>
    /// <typeparam name="T">The type of the data returned by the service action.</typeparam>
    /// <param name="action">The asynchronous method to execute, represented as a function.</param>
    /// <param name="status">The HTTP status code to return on successful execution. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is an
    /// <see cref="ActionResult{T}"/> containing the result of the operation or error details with a corresponding HTTP status code.
    /// </returns>
    protected async Task<ActionResult<T>> ExecuteServiceAsync<T>(Func<Task<T>> action,
        HttpStatusCode status = HttpStatusCode.OK)
    {
        try
        {
            var data = await action.Invoke();
            if (status == HttpStatusCode.NoContent)
            {
                return NoContent();
            }

            return GenerateActionResult(new SbcGenericResponse<T>
            {
                StatusCode = status,
                Success = true,
                Data = data!
            });
        }
        catch (SbcException sbcEx)
        {
            return GenerateActionResult(new SbcGenericResponse
            {
                StatusCode = sbcEx.StatusCode,
                Success = false,
                Message = sbcEx.Message
            });
        }
        catch (Exception e)
        {
            return GenerateActionResult(new SbcGenericResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Success = false,
                Message = e.Message
            });
        }
    }

    private ObjectResult GenerateActionResult(SbcGenericResponse response)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.OK => Ok(response),
            HttpStatusCode.Created => Created("", response),
            HttpStatusCode.NotFound => NotFound(response),
            HttpStatusCode.Unauthorized => Unauthorized(response),
            HttpStatusCode.PreconditionFailed => Problem(response.Message,
                statusCode: Convert.ToInt32(response.StatusCode)),
            _ => BadRequest(response)
        };
    }
}