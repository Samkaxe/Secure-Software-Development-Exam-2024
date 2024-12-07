using Core.Entites;

namespace Application.DTOs;

public class UpdateUserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}