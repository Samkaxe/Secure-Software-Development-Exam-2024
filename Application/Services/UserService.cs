using Application.DTOs;
using Application.Interfaces;
using Core.Entites;
using Infrastructure.DataAccessInterfaces;

namespace Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<UserDTO>> GetAllPatientsAsync()
    {
        var users = await userRepository.GetAllByRoleAsync(Role.Patient);
        return users.Select(user => new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        });
    }
}