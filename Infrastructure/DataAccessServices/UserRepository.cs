using Core.Entites;
using Infrastructure.DataAccessInterfaces;
using Infrastructure.helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessServices;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.MedicalRecords)
            .FirstOrDefaultAsync(u => u.Id == id);
    }




    public async Task<User> GetByEmailAsync(string email)
    {
        if (!ValidatorHelper.IsSqlInjectionSafe(email))
        {
            throw new ArgumentException("Invalid input detected. Potential SQL injection.");
        }

        return await _context.Users
            .Include(u => u.MedicalRecords)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        
        ValidateUser(user);

        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        ValidateUser(user);
        _context.Users.Update(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByResetTokenAsync(string resetToken)
    {
        if (!ValidatorHelper.IsSqlInjectionSafe(resetToken))
        {
            throw new ArgumentException("Invalid input detected. Potential SQL injection.");
        }

        return await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken == resetToken && u.ResetTokenExpiration > DateTime.UtcNow);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.MedicalRecords)
            .ToListAsync();
    }

    private void ValidateUser(User user)
    {
        if (!ValidatorHelper.IsSqlInjectionSafe(user.Email))
        {
            throw new ArgumentException("Invalid email input detected. Potential SQL injection.");
        }

        if (!ValidatorHelper.IsSqlInjectionSafe(user.FirstName))
        {
            throw new ArgumentException("Invalid first name input detected. Potential SQL injection.");
        }

        if (!ValidatorHelper.IsSqlInjectionSafe(user.LastName))
        {
            throw new ArgumentException("Invalid last name input detected. Potential SQL injection.");
        }
    }
}