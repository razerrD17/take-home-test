using Fundo.Services.DTOs;
using Fundo.Services.Enums;
using Fundo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers;

[ApiController]
[Route("loans")]
[Authorize]
public class LoanManagementController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanManagementController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
    {
        var loans = await _loanService.GetAllLoansAsync();
        return Ok(loans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetLoan(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);

        if (loan == null)
        {
            return NotFound(new { message = $"Loan with ID {id} not found" });
        }

        return Ok(loan);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanDto createLoanDto)
    {
        var loan = await _loanService.CreateLoanAsync(createLoanDto);
        return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
    }

    [HttpPost("{id}/payment")]
    public async Task<ActionResult<LoanDto>> MakePayment(int id, [FromBody] PaymentDto paymentDto)
    {
        var result = await _loanService.MakePaymentAsync(id, paymentDto);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ServiceErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                ServiceErrorType.BadRequest => BadRequest(new { message = result.ErrorMessage }),
                _ => BadRequest(new { message = result.ErrorMessage })
            };
        }

        return Ok(result.Data);
    }
}
