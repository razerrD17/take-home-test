using AutoMapper;
using FluentAssertions;
using Fundo.Domain.Entities;
using Fundo.Domain.Interfaces;
using Fundo.Services.DTOs;
using Fundo.Services.Enums;
using Fundo.Services.Implementations;
using Fundo.Services.Mappings;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly LoanService _service;

    public LoanServiceTests()
    {
        _mockRepository = new Mock<ILoanRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<LoanMappingProfile>());
        _mapper = config.CreateMapper();

        _service = new LoanService(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAllLoansAsync_ShouldReturnAllLoans()
    {
        // Arrange
        var loans = new List<Loan>
        {
            new Loan { Id = 1, Amount = 10000m, CurrentBalance = 5000m, ApplicantName = "Test User 1", Status = LoanStatus.Active },
            new Loan { Id = 2, Amount = 5000m, CurrentBalance = 0m, ApplicantName = "Test User 2", Status = LoanStatus.Paid }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(loans);

        // Act
        var result = await _service.GetAllLoansAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoanByIdAsync_WithValidId_ShouldReturnLoan()
    {
        // Arrange
        var loan = new Loan { Id = 1, Amount = 10000m, CurrentBalance = 5000m, ApplicantName = "Test User 1", Status = LoanStatus.Active };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);

        // Act
        var result = await _service.GetLoanByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.ApplicantName.Should().Be("Test User 1");
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetLoanByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Loan?)null);

        // Act
        var result = await _service.GetLoanByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateLoanAsync_ShouldCreateAndReturnLoan()
    {
        // Arrange
        var createDto = new CreateLoanDto
        {
            Amount = 15000m,
            ApplicantName = "New User"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Loan>()))
            .ReturnsAsync((Loan loan) =>
            {
                loan.Id = 1;
                return loan;
            });

        // Act
        var result = await _service.CreateLoanAsync(createDto);

        // Assert
        result.Amount.Should().Be(15000m);
        result.CurrentBalance.Should().Be(15000m);
        result.ApplicantName.Should().Be("New User");
        result.Status.Should().Be(LoanStatus.Active);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Loan>(l =>
            l.Amount == 15000m &&
            l.CurrentBalance == 15000m &&
            l.ApplicantName == "New User" &&
            l.Status == LoanStatus.Active
        )), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_ShouldReduceBalance()
    {
        // Arrange
        var loan = new Loan { Id = 1, Amount = 10000m, CurrentBalance = 5000m, ApplicantName = "Test", Status = LoanStatus.Active };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);

        var paymentDto = new PaymentDto { Amount = 2000m };

        // Act
        var result = await _service.MakePaymentAsync(1, paymentDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.CurrentBalance.Should().Be(3000m);
        result.Data.Status.Should().Be(LoanStatus.Active);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Loan>(l => l.CurrentBalance == 3000m)), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_FullPayment_ShouldSetStatusToPaid()
    {
        // Arrange
        var loan = new Loan { Id = 1, Amount = 5000m, CurrentBalance = 1000m, ApplicantName = "Test", Status = LoanStatus.Active };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Loan>())).Returns(Task.CompletedTask);

        var paymentDto = new PaymentDto { Amount = 1000m };

        // Act
        var result = await _service.MakePaymentAsync(1, paymentDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.CurrentBalance.Should().Be(0);
        result.Data.Status.Should().Be(LoanStatus.Paid);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Loan>(l =>
            l.CurrentBalance == 0 && l.Status == LoanStatus.Paid
        )), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_OnPaidLoan_ShouldReturnBadRequest()
    {
        // Arrange
        var loan = new Loan { Id = 1, Amount = 5000m, CurrentBalance = 0m, ApplicantName = "Test", Status = LoanStatus.Paid };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);

        var paymentDto = new PaymentDto { Amount = 100m };

        // Act
        var result = await _service.MakePaymentAsync(1, paymentDto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.BadRequest);
        result.ErrorMessage.Should().Contain("paid");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Loan>()), Times.Never);
    }

    [Fact]
    public async Task MakePaymentAsync_ExceedingBalance_ShouldReturnBadRequest()
    {
        // Arrange
        var loan = new Loan { Id = 1, Amount = 5000m, CurrentBalance = 1000m, ApplicantName = "Test", Status = LoanStatus.Active };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);

        var paymentDto = new PaymentDto { Amount = 2000m };

        // Act
        var result = await _service.MakePaymentAsync(1, paymentDto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.BadRequest);
        result.ErrorMessage.Should().Contain("exceed");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Loan>()), Times.Never);
    }

    [Fact]
    public async Task MakePaymentAsync_NonExistentLoan_ShouldReturnNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Loan?)null);

        var paymentDto = new PaymentDto { Amount = 100m };

        // Act
        var result = await _service.MakePaymentAsync(999, paymentDto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(ServiceErrorType.NotFound);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Loan>()), Times.Never);
    }
}
