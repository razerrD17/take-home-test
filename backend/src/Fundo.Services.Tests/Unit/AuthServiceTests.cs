using FluentAssertions;
using Fundo.Domain.Entities;
using Fundo.Domain.Interfaces;
using Fundo.Services.DTOs;
using Fundo.Services.Enums;
using Fundo.Services.Implementations;
using Fundo.Services.Interfaces;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtSettings> _mockJwtSettings;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtSettings = new Mock<IJwtSettings>();

        // Setup default JWT settings
        _mockJwtSettings.Setup(s => s.Secret).Returns("ThisIsAVeryLongSecretKeyForTestingPurposes123!");
        _mockJwtSettings.Setup(s => s.Issuer).Returns("TestIssuer");
        _mockJwtSettings.Setup(s => s.Audience).Returns("TestAudience");
        _mockJwtSettings.Setup(s => s.ExpirationInMinutes).Returns(60);

        _service = new AuthService(_mockUserRepository.Object, _mockJwtSettings.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Role = UserRole.User,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var request = new LoginRequest { Username = "testuser", Password = "Password123" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.ExpiresIn.Should().Be(3600); // 60 minutes * 60 seconds
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.LastLoginAt != null)), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldReturnBadRequest()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("nonexistent")).ReturnsAsync((User?)null);

        var request = new LoginRequest { Username = "nonexistent", Password = "Password123" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.BadRequest);
        result.ErrorMessage.Should().Contain("Invalid username or password");
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = passwordHash,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

        var request = new LoginRequest { Username = "testuser", Password = "WrongPassword" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.BadRequest);
        result.ErrorMessage.Should().Contain("Invalid username or password");
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithDisabledUser_ShouldReturnBadRequest()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = passwordHash,
            IsActive = false // Disabled
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

        var request = new LoginRequest { Username = "testuser", Password = "Password123" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.BadRequest);
        result.ErrorMessage.Should().Contain("disabled");
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void HashPassword_ShouldReturnValidBCryptHash()
    {
        // Act
        var hash = _service.HashPassword("TestPassword123");

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2"); // BCrypt hash prefix
        BCrypt.Net.BCrypt.Verify("TestPassword123", hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword("TestPassword123");

        // Act
        var result = _service.VerifyPassword("TestPassword123", hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword("TestPassword123");

        // Act
        var result = _service.VerifyPassword("WrongPassword", hash);

        // Assert
        result.Should().BeFalse();
    }
}
