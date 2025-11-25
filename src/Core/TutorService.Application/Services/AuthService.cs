using Microsoft.Extensions.Logging;
using TutorService.Application.Configuration;
using TutorService.Application.DTOs;
using TutorService.Application.DTOs.Auth;
using TutorService.Application.Intefaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IPasswordService passwordService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.UserExistsAsync(request.Email))
        {
            throw new ArgumentException("User with this email already exists");
        }

        if(!Enum.IsDefined(typeof(UserRole), request.Role))
        {
            throw new ArgumentException("No such role");
        }

        if(request.Role == UserRole.Admin)
        {
            throw new ArgumentException("Admin role is not accessible");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = request.Role,
            IsEmailVerified = false,
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", request.Email);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.Token);
        var userId = Guid.Parse(principal.FindFirst("uid")?.Value);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedRefreshToken == null || storedRefreshToken.UserId != userId || 
            storedRefreshToken.ExpiryDate < DateTime.UtcNow || storedRefreshToken.Used || storedRefreshToken.Invalidated)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        storedRefreshToken.Used = true;
        _refreshTokenRepository.Update(storedRefreshToken);

        return await GenerateAuthResponse(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null)
        {
            return false;
        }

        storedToken.Invalidated = true;
        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = _passwordService.HashPassword(newPassword);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        await _refreshTokenRepository.InvalidateUserTokensAsync(userId);

        return true;
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            CreationDate = DateTime.UtcNow,
            ExpiryDate = _jwtService.GetRefreshTokenExpiryDate(),
            Used = false,
            Invalidated = false,
            UserId = user.Id
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified
            }
        };
    }
}