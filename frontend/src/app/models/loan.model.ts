export interface Loan {
  id: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: string;
}

export interface CreateLoanDto {
  amount: number;
  applicantName: string;
}

export interface PaymentDto {
  amount: number;
}
