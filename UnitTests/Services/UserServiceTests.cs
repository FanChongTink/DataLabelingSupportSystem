using Xunit;
using Moq;
using BLL.Services;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using DTOs.Entities;
using DTOs.Constants;
using System.Threading.Tasks;
using System;

namespace UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup Configuration for Login tests
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["Key"]).Returns("SuperSecretKey12345678901234567890"); // Must be long enough for HMACSHA256
            mockSection.Setup(s => s["Issuer"]).Returns("TestIssuer");
            mockSection.Setup(s => s["Audience"]).Returns("TestAudience");

            _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(mockSection.Object);

            _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ValidData_ReturnsUser()
        {
            // Arrange
            string fullName = "Test User";
            string email = "test@example.com";
            string password = "password123";
            string role = UserRoles.Annotator;

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(email)).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.RegisterAsync(fullName, email, password, role);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(fullName, result.FullName);
            Assert.Equal(role, result.Role);
            Assert.NotNull(result.PasswordHash);
            Assert.NotNull(result.PaymentInfo);
            Assert.Equal(result.Id, result.PaymentInfo.UserId);

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ThrowsException()
        {
             // Arrange
            string fullName = "Test User";
            string email = "existing@example.com";
            string password = "password123";
            string role = UserRoles.Annotator;

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(email)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.RegisterAsync(fullName, email, password, role));
             _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = "user1",
                Email = email,
                PasswordHash = hashedPassword,
                Role = UserRoles.Annotator,
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var token = await _userService.LoginAsync(email, password);

            // Assert
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("otherpassword");

            var user = new User
            {
                Id = "user1",
                Email = email,
                PasswordHash = hashedPassword,
                Role = UserRoles.Annotator,
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var token = await _userService.LoginAsync(email, password);

            // Assert
            Assert.Null(token);
        }
    }
}
