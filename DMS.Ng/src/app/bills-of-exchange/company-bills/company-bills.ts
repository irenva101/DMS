import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BillService } from '../bill.service';
import { Bill, BillStatus } from '../bill.model';
import { companyColor, companyInitials } from '../../contracts/company-logo';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

@Component({
  selector: 'app-company-bills',
  imports: [RouterLink, Pager],
  templateUrl: './company-bills.html',
  styleUrl: './company-bills.scss'
})
export class CompanyBills {
  private readonly service = inject(BillService);

  companyId = input.required<string>();

  protected readonly company = computed(() => this.service.getCompany(this.companyId()));
  protected readonly bills = computed(() => this.service.getBills(this.companyId()));
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly filteredBills = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.bills();
    return this.bills().filter((b) => b.id.toLowerCase().includes(term));
  });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredBills().length / PAGE_SIZE)));

  protected readonly pagedBills = computed(() => paginate(this.filteredBills(), this.page(), PAGE_SIZE));

  constructor() {
    effect(() => {
      if (this.company()) this.service.recordView(this.companyId());
    });
  }

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected status(bill: Bill): BillStatus {
    return this.service.getBillStatus(bill);
  }

  protected statusLabel(status: BillStatus): string {
    return status === 'active' ? 'Aktivna' : status === 'due-soon' ? 'Dospeva uskoro' : 'Dospela';
  }

  protected formatDate(date: Date): string {
    const dd = String(date.getDate()).padStart(2, '0');
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    return `${dd}.${mm}.${date.getFullYear()}.`;
  }

  protected formatAmount(amount: number): string {
    return `${amount.toLocaleString('de-DE')} RSD`;
  }

  protected initials(name: string): string {
    return companyInitials(name);
  }

  protected logoColor(id: string): string {
    return companyColor(id);
  }

  protected download(bill: Bill): void {
    const lines = [
      `Menica: ${bill.id}`,
      `Firma: ${this.company()?.name ?? ''}`,
      `Iznos: ${this.formatAmount(bill.amount)}`,
      `Datum izdavanja: ${this.formatDate(bill.issueDate)}`,
      `Datum dospeća: ${this.formatDate(bill.maturityDate)}`
    ];

    const blob = new Blob([lines.join('\n')], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${bill.id}.txt`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  }
}
