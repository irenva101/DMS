import { Component, Signal, WritableSignal, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BankTransactionService } from '../bank-transaction.service';
import { BankTransaction, BankTransactionType } from '../bank-transaction.model';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

interface TransactionFlag {
  statusClass: 'complete' | 'partial' | 'missing';
  label: string;
}

interface TransactionGroup {
  searchTerm: WritableSignal<string>;
  page: WritableSignal<number>;
  paged: Signal<BankTransaction[]>;
  totalPages: Signal<number>;
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

  protected readonly domestic = this.createGroup('domestic');
  protected readonly foreign = this.createGroup('foreign');

  protected onSearch(group: TransactionGroup, value: string): void {
    group.searchTerm.set(value);
    group.page.set(1);
  }

  protected flag(transaction: BankTransaction): TransactionFlag {
    const missing = transaction.documents.filter((d) => d.status === 'missing').length;
    if (missing === 0) return { statusClass: 'complete', label: 'Kompletno' };
    if (missing === transaction.documents.length) {
      return { statusClass: 'missing', label: 'Nijedan dokument nije predat' };
    }
    return { statusClass: 'partial', label: `${missing} od ${transaction.documents.length} nedostaje` };
  }

  private createGroup(type: BankTransactionType): TransactionGroup {
    const searchTerm = signal('');
    const page = signal(1);

    const filtered = computed(() => {
      const term = searchTerm().trim().toLowerCase();
      return this.transactions().filter(
        (t) => t.type === type && (!term || t.company.toLowerCase().includes(term) || t.id.toLowerCase().includes(term))
      );
    });

    const totalPages = computed(() => Math.max(1, Math.ceil(filtered().length / PAGE_SIZE)));
    const paged = computed(() => paginate(filtered(), page(), PAGE_SIZE));

    return { searchTerm, page, paged, totalPages };
  }
}
