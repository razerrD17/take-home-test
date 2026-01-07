import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loan, CreateLoanDto, PaymentDto } from '../models/loan.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/loans`;

  getLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(this.apiUrl);
  }

  getLoan(id: number): Observable<Loan> {
    return this.http.get<Loan>(`${this.apiUrl}/${id}`);
  }

  createLoan(loan: CreateLoanDto): Observable<Loan> {
    return this.http.post<Loan>(this.apiUrl, loan);
  }

  makePayment(id: number, payment: PaymentDto): Observable<Loan> {
    return this.http.post<Loan>(`${this.apiUrl}/${id}/payment`, payment);
  }
}
