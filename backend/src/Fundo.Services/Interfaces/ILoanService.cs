using Fundo.Services.Common;
using Fundo.Services.DTOs;

namespace Fundo.Services.Interfaces;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync();
    Task<LoanDto?> GetLoanByIdAsync(int id);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
    Task<ServiceResult<LoanDto>> MakePaymentAsync(int id, PaymentDto paymentDto);
}
