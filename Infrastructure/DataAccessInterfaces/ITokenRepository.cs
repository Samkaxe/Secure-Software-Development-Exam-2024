using Core.Entites;

namespace Infrastructure.DataAccessInterfaces;

public interface ITokenRepository
{
    Task<Token> GetByIdAsync(Guid id);
    Task<Token> GetByUserIdAsync(Guid userId);
    Task AddAsync(Token token);
    Task UpdateAsync(Token token);
    Task DeleteAsync(Token token);
    Task SaveChangesAsync();
}