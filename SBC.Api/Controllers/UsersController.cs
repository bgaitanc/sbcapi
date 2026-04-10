using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Application.Models.Common;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Interfaces;

namespace SBC.Api.Controllers;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
[Authorize]
public class UsersController(IUserService userService) : SbcControllerBase
{
    /// <summary>
    /// Creates a new user with the specified details and roles.
    /// </summary>
    /// <param name="createUserDto">The user details and roles.</param>
    /// <returns>The unique identifier of the created user.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SbcGenericResponse<Guid>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        return await ExecuteServiceAsync(async () => await userService.CreateUserAsync(createUserDto),
            HttpStatusCode.Created);
    }

    /// <summary>
    /// Updates an existing user with the specified details and roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="updateUserDto">The updated user details and roles.</param>
    /// <returns>A status indicating success.</returns>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<SbcGenericResponse>> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
    {
        return await ExecuteServiceAsync(async () => await userService.UpdateUserAsync(userId, updateUserDto));
    }

    /// <summary>
    /// Updates the password for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose password is being updated.</param>
    /// <param name="updatePasswordDto">The current and new passwords.</param>
    /// <returns>A status indicating success.</returns>
    [HttpPut("{userId:guid}/password")]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<SbcGenericResponse>> UpdatePassword(Guid userId, [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        return await ExecuteServiceAsync(async () => await userService.UpdatePasswordAsync(userId, updatePasswordDto));
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SbcGenericResponse<PagedResultDto<UserDto>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetUsers([FromQuery] UserFilterDto filter)
    {
        if (filter.PageSize == 0 && filter.PageNumber == 0)
        {
            return await ExecuteServiceAsync(async () =>
            {
                var users = await userService.GetUsersAsync();
                return new PagedResultDto<UserDto>
                {
                    Items = users,
                    TotalCount = users.Count(),
                    PageNumber = 1,
                    PageSize = users.Count() == 0 ? 10 : users.Count()
                };
            });
        }
        return await ExecuteServiceAsync(async () => await userService.GetPagedUsersAsync(filter));
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The details of the user.</returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(SbcGenericResponse<UserDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        return await ExecuteServiceAsync(async () => await userService.GetUserByIdAsync(userId));
    }
}
