import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ContractService } from '../contract.service';
import { Company } from '../contract.model';
import { companyColor, companyInitials } from '../company-logo';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

@Component({
  selector: 'app-company-list',
  imports: [RouterLink, Pager],
  templateUrl: './company-list.html',
  styleUrl: './company-list.scss'
})
export class CompanyList {
  private readonly service = inject(ContractService);

  protected readonly companies = signal<Company[]>(this.service.getCompanies());
  protected readonly recentlyViewed = signal<Company[]>(this.service.getRecentlyViewed());
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly filteredCompanies = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.companies();
    return this.companies().filter((c) => c.name.toLowerCase().includes(term));
  });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredCompanies().length / PAGE_SIZE)));

  protected readonly pagedCompanies = computed(() => paginate(this.filteredCompanies(), this.page(), PAGE_SIZE));

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected contractCount(companyId: string): number {
    return this.service.getContracts(companyId).length;
  }

  protected initials(name: string): string {
    return companyInitials(name);
  }

  protected logoColor(id: string): string {
    return companyColor(id);
  }
}
