namespace Fundo.Services.DTOs;

public class CreateLoanDto
{
    public decimal Amount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
}
