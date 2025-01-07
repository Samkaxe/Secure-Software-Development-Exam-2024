using Application.DTOs;
using Infrastructure.DataAccessInterfaces;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllPatientsAsync();
}