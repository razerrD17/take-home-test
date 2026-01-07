namespace Fundo.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public decimal CurrentBalance { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = LoanStatus.Active;
}

public static class LoanStatus
{
    public const string Active = "active";
    public const string Paid = "paid";
}
