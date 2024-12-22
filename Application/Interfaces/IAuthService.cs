using Application.DTOs;
using Core.Entites;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenDTO> LoginAsync(string email, string password);
    Task LogoutAsync(Guid userId);
    
    // CRUD Operations
    Task<UserDTO> CreateUserAsync(CreateUserDTO userDto);
    Task<UserDTO> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(Guid userId, UpdateUserDTO userDto);
    Task<bool> DeleteUserAsync(Guid userId);
}