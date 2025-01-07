using Core.Entites;

namespace Infrastructure.DataAccessInterfaces;


public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
    Task<User> GetByResetTokenAsync(string resetToken);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetAllByRoleAsync(Role role);
}