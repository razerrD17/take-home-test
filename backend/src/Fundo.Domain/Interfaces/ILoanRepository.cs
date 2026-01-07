using Fundo.Domain.Entities;

namespace Fundo.Domain.Interfaces;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(int id);
    Task<Loan> AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
}
