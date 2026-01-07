using AutoMapper;
using Fundo.Domain.Entities;
using Fundo.Domain.Interfaces;
using Fundo.Services.Common;
using Fundo.Services.DTOs;
using Fundo.Services.Interfaces;

namespace Fundo.Services.Implementations;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IMapper _mapper;

    public LoanService(ILoanRepository loanRepository, IMapper mapper)
    {
        _loanRepository = loanRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
    {
        var loans = await _loanRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<LoanDto>>(loans);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(int id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        return loan == null ? null : _mapper.Map<LoanDto>(loan);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        var loan = _mapper.Map<Loan>(createLoanDto);

        await _loanRepository.AddAsync(loan);
        return _mapper.Map<LoanDto>(loan);
    }

    public async Task<ServiceResult<LoanDto>> MakePaymentAsync(int id, PaymentDto paymentDto)
    {
        var loan = await _loanRepository.GetByIdAsync(id);

        if (loan == null)
        {
            return ServiceResult<LoanDto>.NotFound($"Loan with ID {id} not found");
        }

        if (loan.Status == LoanStatus.Paid)
        {
            return ServiceResult<LoanDto>.BadRequest("This loan has already been paid off");
        }

        if (paymentDto.Amount > loan.CurrentBalance)
        {
            return ServiceResult<LoanDto>.BadRequest(
                $"Payment amount ({paymentDto.Amount:C}) exceeds current balance ({loan.CurrentBalance:C})");
        }

        loan.CurrentBalance -= paymentDto.Amount;

        if (loan.CurrentBalance == 0)
        {
            loan.Status = LoanStatus.Paid;
        }

        await _loanRepository.UpdateAsync(loan);
        return ServiceResult<LoanDto>.Ok(_mapper.Map<LoanDto>(loan));
    }
}
