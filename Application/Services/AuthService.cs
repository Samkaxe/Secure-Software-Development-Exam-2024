﻿using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Core.Entites;
using Infrastructure.DataAccessInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly EncryptionHelper _encryptionHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor, EncryptionHelper encryptionHelper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _encryptionHelper = encryptionHelper;
        _configuration = configuration;
    }

    public async Task<TokenDTO> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordPepper = FetchPasswordPepper();
        var isPasswordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt, passwordPepper);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");
        
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.ToString());
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var derivedKey = _encryptionHelper.DeriveKey(password, user.PasswordSalt);
            httpContext.Session.Set("uek", derivedKey);
        }
        
        return new TokenDTO
        {
            AccessToken = accessToken,
            TokenExpiration = DateTime.UtcNow.AddHours(1)
        };
    }

    private string FetchPasswordPepper()
    {
        return _configuration.GetSection("PasswordPepper").Value!;
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserDTO> CreateUserAsync(CreateUserDTO userDto)
    {
        var salt = PasswordHelper.GenerateSalt();
        var pepper = FetchPasswordPepper();
        var hashedPassword = PasswordHelper.HashPassword(userDto.Password, salt, pepper);

        var encryptedUeK = _encryptionHelper.EncryptWithMasterKey(
            _encryptionHelper.DeriveKey(userDto.Password, salt)
        );

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            PasswordHash = hashedPassword,
            PasswordSalt = salt,
            Role = userDto.Role,
            EncryptedUek = encryptedUeK,
            ResetToken = "123" // temporary 
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