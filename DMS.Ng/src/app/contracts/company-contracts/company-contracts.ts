import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ContractService } from '../contract.service';
import { Contract, ContractStatus } from '../contract.model';
import { companyColor, companyInitials } from '../company-logo';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

@Component({
  selector: 'app-company-contracts',
  imports: [RouterLink, Pager],
  templateUrl: './company-contracts.html',
  styleUrl: './company-contracts.scss'
})
export class CompanyContracts {
  private readonly service = inject(ContractService);

  companyId = input.required<string>();

  protected readonly company = computed(() => this.service.getCompany(this.companyId()));
  protected readonly contracts = computed(() => this.service.getContracts(this.companyId()));
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly filteredContracts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.contracts();
    return this.contracts().filter(
      (c) => c.title.toLowerCase().includes(term) || c.id.toLowerCase().includes(term)
    );
  });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredContracts().length / PAGE_SIZE)));

  protected readonly pagedContracts = computed(() => paginate(this.filteredContracts(), this.page(), PAGE_SIZE));

  constructor() {
    effect(() => {
      if (this.company()) this.service.recordView(this.companyId());
    });
  }

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected status(contract: Contract): ContractStatus {
    return this.service.getContractStatus(contract);
  }

  protected statusLabel(status: ContractStatus): string {
    return status === 'active' ? 'Aktivan' : status === 'expiring' ? 'Ističe uskoro' : 'Istekao';
  }

  protected formatDate(date: Date): string {
    const dd = String(date.getDate()).padStart(2, '0');
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    return `${dd}.${mm}.${date.getFullYear()}.`;
  }

  protected formatValue(value: number): string {
    return `${value.toLocaleString('de-DE')} €`;
  }

  protected initials(name: string): string {
    return companyInitials(name);
  }

  protected logoColor(id: string): string {
    return companyColor(id);
  }

  protected download(contract: Contract): void {
    const lines = [
      `Ugovor: ${contract.title}`,
      `Broj: ${contract.id}`,
      `Firma: ${this.company()?.name ?? ''}`,
      `Datum potpisivanja: ${this.formatDate(contract.signedDate)}`,
      `Datum isteka: ${this.formatDate(contract.expiryDate)}`,
      `Vrednost: ${this.formatValue(contract.value)}`
    ];

    const blob = new Blob([lines.join('\n')], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${contract.id}.txt`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  }
}
