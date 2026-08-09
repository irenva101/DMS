import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BankTransactionService } from '../bank-transaction.service';
import { BankTransaction } from '../bank-transaction.model';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

interface TransactionFlag {
  statusClass: 'complete' | 'partial' | 'missing';
  label: string;
}

@Component({
  selector: 'app-bank-transaction-list',
  imports: [RouterLink, Pager],
  templateUrl: './bank-transaction-list.html',
  styleUrl: './bank-transaction-list.scss'
})
export class BankTransactionList {
  private readonly service = inject(BankTransactionService);

  protected readonly transactions = signal<BankTransaction[]>(this.service.getTransactions());
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly filteredTransactions = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.transactions();
    return this.transactions().filter(
      (t) => t.company.toLowerCase().includes(term) || t.id.toLowerCase().includes(term)
    );
  });

  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filteredTransactions().length / PAGE_SIZE))
  );

  protected readonly pagedTransactions = computed(() =>
    paginate(this.filteredTransactions(), this.page(), PAGE_SIZE)
  );

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected flag(transaction: BankTransaction): TransactionFlag {
    const missing = transaction.documents.filter((d) => d.status === 'missing').length;
    if (missing === 0) return { statusClass: 'complete', label: 'Kompletno' };
    if (missing === transaction.documents.length) {
      return { statusClass: 'missing', label: 'Nijedan dokument nije predat' };
    }
    return { statusClass: 'partial', label: `${missing} od ${transaction.documents.length} nedostaje` };
  }
}
