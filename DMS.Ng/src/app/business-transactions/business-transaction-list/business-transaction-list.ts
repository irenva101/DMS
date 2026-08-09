import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusDot } from '../status-dot/status-dot';
import { BusinessTransactionService } from '../business-transaction.service';
import { BusinessTransaction } from '../business-transaction.model';

interface TransactionFlag {
  statusClass: 'missing' | 'pending' | 'done';
  label: string;
}

@Component({
  selector: 'app-business-transaction-list',
  imports: [RouterLink, StatusDot],
  templateUrl: './business-transaction-list.html',
  styleUrl: './business-transaction-list.scss'
})
export class BusinessTransactionList {
  private readonly service = inject(BusinessTransactionService);

  protected readonly transactions = signal<BusinessTransaction[]>(this.service.getTransactions());

  protected readonly stats = computed(() => {
    let missingDocument = 0;
    let pendingVerification = 0;
    let completed = 0;

    for (const transaction of this.transactions()) {
      const status = this.flag(transaction).statusClass;
      if (status === 'missing') missingDocument++;
      else if (status === 'pending') pendingVerification++;
      else completed++;
    }

    return { missingDocument, pendingVerification, completed };
  });

  protected flag(transaction: BusinessTransaction): TransactionFlag {
    const missing = transaction.steps.filter((s) => s.status === 'missing').length;
    const pending = transaction.steps.filter((s) => s.status === 'pending').length;

    if (missing > 0) {
      return { statusClass: 'missing', label: `${missing} ${missing === 1 ? 'dokument nedostaje' : 'dokumenta nedostaje'}` };
    }
    if (pending > 0) {
      return { statusClass: 'pending', label: 'čeka verifikaciju' };
    }
    return { statusClass: 'done', label: 'kompletirano' };
  }
}
