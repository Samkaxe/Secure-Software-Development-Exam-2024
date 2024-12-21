using Application.DTOs;
using Application.Interfaces;
using Core.Entites;
using Infrastructure.DataAccessInterfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<TokenDTO> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var isPasswordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");
        
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        var token = new Token
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(1), // Move this value to configuration
            CreatedAt = DateTime.UtcNow,
            DeviceInfo = "Unknown Device" // Use input parameter instead of hardcoding
        };

        user.Token = token;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new TokenDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddHours(1) // Move this to configuration if necessary
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        user.Token = null;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserDTO> CreateUserAsync(CreateUserDTO userDto)
    {
        var salt = PasswordHelper.GenerateSalt();
        var hashedPassword = PasswordHelper.HashPassword(userDto.Password, salt);

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            PasswordHash = hashedPassword,
            PasswordSalt = salt,
            Role = userDto.Role
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<UserDTO> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(user => new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        });
    }

    public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDTO userDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Email = userDto.Email;
        user.Role = userDto.Role;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}