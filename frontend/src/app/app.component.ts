import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoanService } from './services/loan.service';
import { Loan } from './models/loan.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  private loanService = inject(LoanService);

  displayedColumns: string[] = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
  ];

  loans: Loan[] = [];
  loading = true;
  error: string | null = null;

  ngOnInit(): void {
    this.loadLoans();
  }

  loadLoans(): void {
    this.loading = true;
    this.error = null;

    this.loanService.getLoans().subscribe({
      next: (loans) => {
        this.loans = loans;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading loans:', err);
        this.error = 'Failed to load loans. Please ensure the backend server is running.';
        this.loading = false;
      }
    });
  }
}
