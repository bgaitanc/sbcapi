using SBC.Application.Models.Auth;
using SBC.Application.Models.Common;

namespace SBC.Application.Services.Interfaces;

public interface IUserService
{
    Task<Guid> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
    Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePasswordDto);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<PagedResultDto<UserDto>> GetPagedUsersAsync(UserFilterDto filter);
    Task<UserDto> GetUserByIdAsync(Guid userId);
}
