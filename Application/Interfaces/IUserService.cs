using Application.DTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync(Guid userId);
    
    // CRUD Operations
    Task<UserDTO> CreateUserAsync(CreateUserDTO userDto);
    Task<UserDTO> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(Guid userId, UpdateUserDTO userDto);
    Task<bool> DeleteUserAsync(Guid userId);
}