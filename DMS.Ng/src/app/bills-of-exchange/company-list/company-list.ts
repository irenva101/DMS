import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BillService } from '../bill.service';
import { Company } from '../bill.model';
import { companyColor, companyInitials } from '../../contracts/company-logo';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';
import { ActiveMeniceList } from '../active-menice-list/active-menice-list';

const PAGE_SIZE = 3;

@Component({
  selector: 'app-bill-company-list',
  imports: [RouterLink, Pager, ActiveMeniceList],
  templateUrl: './company-list.html',
  styleUrl: './company-list.scss'
})
export class CompanyList {
  private readonly service = inject(BillService);

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

  protected billCount(companyId: string): number {
    return this.service.getBills(companyId).length;
  }

  protected initials(name: string): string {
    return companyInitials(name);
  }

  protected logoColor(id: string): string {
    return companyColor(id);
  }
}
