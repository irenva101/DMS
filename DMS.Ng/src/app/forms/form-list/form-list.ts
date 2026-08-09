import { Component, computed, inject, signal } from '@angular/core';
import { FormTemplateService } from '../form-template.service';
import { FormTemplate } from '../form-template.model';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 7;

@Component({
  selector: 'app-form-list',
  imports: [Pager],
  templateUrl: './form-list.html',
  styleUrl: './form-list.scss'
})
export class FormList {
  private readonly service = inject(FormTemplateService);

  protected readonly forms = signal<FormTemplate[]>(this.service.getForms());
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly filteredForms = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.forms();
    return this.forms().filter((f) => f.name.toLowerCase().includes(term));
  });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredForms().length / PAGE_SIZE)));

  protected readonly pagedForms = computed(() => paginate(this.filteredForms(), this.page(), PAGE_SIZE));

  protected readonly groupedForms = computed(() => {
    const groups = new Map<string, FormTemplate[]>();
    for (const form of this.pagedForms()) {
      const items = groups.get(form.category) ?? [];
      items.push(form);
      groups.set(form.category, items);
    }
    return Array.from(groups.entries()).map(([category, items]) => ({ category, items }));
  });

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected download(form: FormTemplate): void {
    const lines = [`Obrazac: ${form.name}`, `Kategorija: ${form.category}`, `Oznaka: ${form.id}`];

    const blob = new Blob([lines.join('\n')], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${form.id}.txt`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  }
}
