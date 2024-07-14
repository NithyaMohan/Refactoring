using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace LegacyApp.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUserCreditService> _userCreditServiceMock;
        private readonly Mock<IUserDataAccessProxy> _userDataAccessMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userCreditServiceMock = new Mock<IUserCreditService>();
            _userDataAccessMock = new Mock<IUserDataAccessProxy>();

            _userService = new UserService(
                _clientRepositoryMock.Object,
                _userCreditServiceMock.Object,
                _userDataAccessMock.Object
            );
        }

        [Fact]
        public async Task AddUser_ValidUser_ReturnsUser()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "NormalClient" };
            _clientRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(client);
            _userCreditServiceMock.Setup(service => service.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(1000);

            // Act
            var result = await _userService.AddUser("John", "Doe", "john.doe@example.com", new DateTime(1990, 1, 1), 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Firstname);
            _userDataAccessMock.Verify(da => da.AddUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddUser_InvalidEmail_ThrowsInvalidOperationException()
        {
            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("John", "Doe", "invalid-email", new DateTime(1990, 1, 1), 1));
            Assert.Equal("user email is invalid ", ex.Message);
        }

        [Fact]
        public async Task AddUser_TooYoung_ThrowsInvalidOperationException()
        {
            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("John", "Doe", "john.doe@example.com", DateTime.Now.AddYears(-20), 1));
            Assert.Equal("user should be older than 21 years", ex.Message);
        }

        [Fact]
        public async Task AddUser_LowCreditLimit_ThrowsInvalidOperationException()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "NormalClient" };
            _clientRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(client);
            _userCreditServiceMock.Setup(service => service.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(400);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("John", "Doe", "john.doe@example.com", new DateTime(1990, 1, 1), 1));
            Assert.Equal("insufficient credit limit", ex.Message);
            _userDataAccessMock.Verify(da => da.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUser_VeryImportantClient_NoCreditLimitCheck_ReturnsUser()
        {
            // Arrange
            var client = new Client { Id = 2, Name = "VeryImportantClient" };
            _clientRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(client);

            // Act
            var result = await _userService.AddUser("Jane", "Doe", "jane.doe@example.com", new DateTime(1990, 1, 1), 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.Firstname);
            Assert.False(result.HasCreditLimit);
            _userDataAccessMock.Verify(da => da.AddUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddUser_ImportantClient_DoubleCreditLimit_ReturnsUser()
        {
            // Arrange
            var client = new Client { Id = 3, Name = "ImportantClient" };
            _clientRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(client);
            _userCreditServiceMock.Setup(service => service.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(600);

            // Act
            var result = await _userService.AddUser("Jake", "Doe", "jake.doe@example.com", new DateTime(1990, 1, 1), 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jake", result.Firstname);
            Assert.Equal(1200, result.CreditLimit);
            _userDataAccessMock.Verify(da => da.AddUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddUser_MissingFirstNameOrSurname_ThrowsInvalidOperationException()
        {
            // Act and Assert
            var ex1 = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("", "Doe", "jane.doe@example.com", new DateTime(1990, 1, 1), 1));
            Assert.Equal("user firstname / surname is required ", ex1.Message);

            var ex2 = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("Jane", "", "jane.doe@example.com", new DateTime(1990, 1, 1), 1));
            Assert.Equal("user firstname / surname is required ", ex2.Message);
        }

        [Fact]
        public async Task AddUser_InvalidClientId_ThrowsInvalidOperationException()
        {
            // Arrange
            _clientRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Client?)null);

            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("John", "Doe", "john.doe@example.com", new DateTime(1990, 1, 1), -1));
            Assert.Equal("Client with this Id does not exist.", ex.Message);
        }

        [Theory]
        [InlineData("johndoecom")]
        [InlineData("john!doecom")]
        public async Task AddUser_InvalidEmailFormats_ThrowsInvalidOperationException(string email)
        {
            // Act and Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.AddUser("John", "Doe", email, new DateTime(1990, 1, 1), 1));
            Assert.Equal("user email is invalid ", ex.Message);
        }
    }
}
