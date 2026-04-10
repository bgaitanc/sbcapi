using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBC.Application.Models.Common;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Identity;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Exceptions;

namespace SBC.Application.Services.Implementation;

public class UserService(
    UserManager<ApplicationUser> userManager,
    ITransactionLogService transactionLogService)
    : IUserService
{
    public async Task<Guid> CreateUserAsync(CreateUserDto createUserDto)
    {
        var emailExists = await userManager.FindByEmailAsync(createUserDto.Email);
        if (emailExists != null)
        {
            var error = "El correo electrónico ya está registrado.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CreateUser, nameof(ApplicationUser), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { createUserDto.UserName, createUserDto.Email }), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        var userNameExists = await userManager.FindByNameAsync(createUserDto.UserName);
        if (userNameExists != null)
        {
            var error = "El nombre de usuario ya está registrado.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CreateUser, nameof(ApplicationUser), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { createUserDto.UserName, createUserDto.Email }), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        var user = new ApplicationUser
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };

        var result = await userManager.CreateAsync(user, createUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            var error = $"Error al crear usuario: {errors}";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CreateUser, nameof(ApplicationUser), null, TransactionStatus.Failure, JsonSerializer.Serialize(new { createUserDto.UserName, createUserDto.Email }), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        if (createUserDto.Roles != null && createUserDto.Roles.Count != 0)
        {
            var roleResult = await userManager.AddToRolesAsync(user, createUserDto.Roles);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                var error = $"Usuario creado pero hubo errores al asignar roles: {errors}";
                await transactionLogService.LogTransactionAsync(user.Id, TransactionActions.CreateUser, nameof(ApplicationUser), user.Id.ToString(), TransactionStatus.Failure, JsonSerializer.Serialize(new { createUserDto.UserName, createUserDto.Email }), error);
                throw new SbcException(HttpStatusCode.PreconditionFailed, error);
            }
        }
        else
        {
            await userManager.AddToRoleAsync(user, "Guest");
        }

        await transactionLogService.LogTransactionAsync(user.Id, TransactionActions.CreateUser, nameof(ApplicationUser), user.Id.ToString(), TransactionStatus.Success, JsonSerializer.Serialize(new { createUserDto.UserName, createUserDto.Email }));

        return user.Id;
    }

    public async Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new SbcException(HttpStatusCode.NotFound, "Usuario no encontrado.");
        }

        var emailExists = await userManager.FindByEmailAsync(updateUserDto.Email);
        if (emailExists != null && emailExists.Id != userId)
        {
            var error = "El correo electrónico ya está registrado por otro usuario.";
            await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdateUser, nameof(ApplicationUser), userId.ToString(), TransactionStatus.ValidationError, JsonSerializer.Serialize(updateUserDto), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        var userNameExists = await userManager.FindByNameAsync(updateUserDto.UserName);
        if (userNameExists != null && userNameExists.Id != userId)
        {
            var error = "El nombre de usuario ya está registrado por otro usuario.";
            await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdateUser, nameof(ApplicationUser), userId.ToString(), TransactionStatus.ValidationError, JsonSerializer.Serialize(updateUserDto), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        user.UserName = updateUserDto.UserName;
        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            var error = $"Error al actualizar usuario: {errors}";
            await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdateUser, nameof(ApplicationUser), userId.ToString(), TransactionStatus.Failure, JsonSerializer.Serialize(updateUserDto), error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        if (updateUserDto.Roles != null)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            
            if (updateUserDto.Roles.Count != 0)
            {
                await userManager.AddToRolesAsync(user, updateUserDto.Roles);
            }
            else
            {
                await userManager.AddToRoleAsync(user, "Guest");
            }
        }

        await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdateUser, nameof(ApplicationUser), userId.ToString(), TransactionStatus.Success, JsonSerializer.Serialize(updateUserDto));
    }

    public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePasswordDto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new SbcException(HttpStatusCode.NotFound, "Usuario no encontrado.");
        }

        var result = await userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            var error = $"Error al cambiar contraseña: {errors}";
            await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdatePassword, nameof(ApplicationUser), userId.ToString(), TransactionStatus.Failure, null, error);
            throw new SbcException(HttpStatusCode.PreconditionFailed, error);
        }

        await transactionLogService.LogTransactionAsync(userId, TransactionActions.UpdatePassword, nameof(ApplicationUser), userId.ToString(), TransactionStatus.Success);
    }

    public async Task<PagedResultDto<UserDto>> GetPagedUsersAsync(UserFilterDto filter)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.UserName))
            query = query.Where(u => u.UserName!.Contains(filter.UserName));

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(u => u.Email!.Contains(filter.Email));

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(u => u.FirstName!.Contains(filter.FirstName));

        if (!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(u => u.LastName!.Contains(filter.LastName));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in items)
        {
            var roles = await userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto(user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName, roles.ToList()));
        }

        return new PagedResultDto<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto(user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName, roles.ToList()));
        }

        return userDtos;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new SbcException(HttpStatusCode.NotFound, "Usuario no encontrado.");
        }

        var roles = await userManager.GetRolesAsync(user);
        return new UserDto(user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName, roles.ToList());
    }
}
