using Core.Entites;
using Infrastructure.DataAccessInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessServices;

public class TokenRepository : ITokenRepository
{
    private readonly ApplicationDbContext _context;

    public TokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Token> GetByIdAsync(Guid id)
    {
        return await _context.Tokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Token> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tokens
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task AddAsync(Token token)
    {
        await _context.Tokens.AddAsync(token);
    }

    public async Task UpdateAsync(Token token)
    {
        _context.Tokens.Update(token);
    }

    public async Task DeleteAsync(Token token)
    {
        _context.Tokens.Remove(token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
