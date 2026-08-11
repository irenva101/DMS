import { Component, Signal, WritableSignal, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DistributionCaseService } from '../distribution-case.service';
import { DistributionCase, DistributionCustomerType } from '../distribution-case.model';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 3;

interface CaseFlag {
  statusClass: 'complete' | 'partial' | 'missing';
  label: string;
}

interface CaseGroup {
  searchTerm: WritableSignal<string>;
  page: WritableSignal<number>;
  paged: Signal<DistributionCase[]>;
  totalPages: Signal<number>;
}

@Component({
  selector: 'app-distribution-case-list',
  imports: [RouterLink, Pager],
  templateUrl: './distribution-case-list.html',
  styleUrl: './distribution-case-list.scss'
})
export class DistributionCaseList {
  private readonly service = inject(DistributionCaseService);

  protected readonly cases = signal<DistributionCase[]>(this.service.getCases());

  protected readonly domestic = this.createGroup('domestic');
  protected readonly foreign = this.createGroup('foreign');

  protected onSearch(group: CaseGroup, value: string): void {
    group.searchTerm.set(value);
    group.page.set(1);
  }

  protected flag(distributionCase: DistributionCase): CaseFlag {
    const documents = distributionCase.steps.flatMap((s) => s.documents);
    const missing = documents.filter((d) => d.status === 'missing').length;
    if (missing === 0) return { statusClass: 'complete', label: 'Kompletno' };
    if (missing === documents.length) return { statusClass: 'missing', label: 'Nijedan dokument nije predat' };
    return { statusClass: 'partial', label: `${missing} od ${documents.length} nedostaje` };
  }

  private createGroup(type: DistributionCustomerType): CaseGroup {
    const searchTerm = signal('');
    const page = signal(1);

    const filtered = computed(() => {
      const term = searchTerm().trim().toLowerCase();
      return this.cases().filter(
        (c) => c.customerType === type && (!term || c.company.toLowerCase().includes(term) || c.id.toLowerCase().includes(term))
      );
    });

    const totalPages = computed(() => Math.max(1, Math.ceil(filtered().length / PAGE_SIZE)));
    const paged = computed(() => paginate(filtered(), page(), PAGE_SIZE));

    return { searchTerm, page, paged, totalPages };
  }
}
