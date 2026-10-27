import { Component, computed, inject, signal } from '@angular/core';
import { ApiException, Client, FormTemplateDataOut } from '../../api/api-reference';
import { Pager } from '../../shared/pager/pager';
import { paginate } from '../../shared/pager/paginate';

const PAGE_SIZE = 7;
// Same as FileRules.MaxFileSizeBytes on the backend
const MAX_FILE_SIZE_BYTES = 50 * 1024 * 1024;
const FILE_TOO_LARGE = 'Fajl je prevelik. Najveća dozvoljena veličina je 50 MB.';

@Component({
  selector: 'app-form-list',
  imports: [Pager],
  templateUrl: './form-list.html',
  styleUrl: './form-list.scss'
})
export class FormList {
  private readonly client = inject(Client);

  protected readonly forms = signal<FormTemplateDataOut[]>([]);
  protected readonly loadFailed = signal(false);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);

  protected readonly adding = signal(false);
  protected readonly saving = signal(false);
  protected readonly saveError = signal<string | null>(null);
  protected readonly newName = signal('');
  protected readonly newCategory = signal('');
  protected readonly newFile = signal<File | null>(null);
  protected readonly dragOver = signal(false);

  // Same list as FileRules.AllowedExtensions on the backend
  protected readonly acceptedFiles =
    '.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.odt,.ods,.csv,.txt,.xml,.png,.jpg,.jpeg,.gif,.bmp,.tif,.tiff,.zip,.rar,.7z,.msg,.eml';

  protected readonly categories = computed(() => [...new Set(this.forms().map((f) => f.category ?? ''))]);

  protected readonly filteredForms = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return this.forms();
    return this.forms().filter((f) => (f.name ?? '').toLowerCase().includes(term));
  });

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.filteredForms().length / PAGE_SIZE)));

  protected readonly pagedForms = computed(() => paginate(this.filteredForms(), this.page(), PAGE_SIZE));

  protected readonly groupedForms = computed(() => {
    const groups = new Map<string, FormTemplateDataOut[]>();
    for (const form of this.pagedForms()) {
      const category = form.category ?? '';
      const items = groups.get(category) ?? [];
      items.push(form);
      groups.set(category, items);
    }
    return Array.from(groups.entries()).map(([category, items]) => ({ category, items }));
  });

  constructor() {
    this.load();
  }

  private load(): void {
    this.client.formsAll().subscribe({
      next: (forms) => {
        this.forms.set(forms);
        this.loadFailed.set(false);
      },
      error: () => this.loadFailed.set(true)
    });
  }

  protected onSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected openAdd(): void {
    this.newName.set('');
    this.newCategory.set('');
    this.newFile.set(null);
    this.saveError.set(null);
    this.adding.set(true);
  }

  protected onDrop(event: DragEvent): void {
    event.preventDefault();
    this.dragOver.set(false);
    this.newFile.set(event.dataTransfer?.files[0] ?? null);
  }

  protected onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.newFile.set(input.files?.[0] ?? null);
  }

  protected formatSize(bytes: number): string {
    if (bytes < 1024 * 1024) return `${Math.max(1, Math.round(bytes / 1024))} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1).replace('.', ',')} MB`;
  }

  protected save(): void {
    const file = this.newFile();
    if (!this.newName().trim() || !this.newCategory().trim() || !file) {
      this.saveError.set('Popuni naziv i kategoriju i izaberi fajl.');
      return;
    }

    // Checked here because the browser would upload the whole file before showing the server's rejection
    if (file.size > MAX_FILE_SIZE_BYTES) {
      this.saveError.set(FILE_TOO_LARGE);
      return;
    }

    this.saving.set(true);
    this.saveError.set(null);
    this.client.formsPOST(this.newName(), this.newCategory(), { data: file, fileName: file.name }).subscribe({
      next: () => {
        this.saving.set(false);
        this.adding.set(false);
        this.load();
      },
      error: (error: unknown) => {
        this.saving.set(false);
        // 413 comes from nginx or Kestrel before the request reaches the API
        this.saveError.set(
          error instanceof ApiException && error.status === 413
            ? FILE_TOO_LARGE
            :'Obrazac nije sačuvan. Fajl mora biti dozvoljenog tipa i manji od 50 MB.'
        );
      }
    });
  }

  protected remove(form: FormTemplateDataOut): void {
    if (!confirm(`Obrisati obrazac „${form.name}“?`)) return;

    this.client.formsDELETE(form.code!).subscribe({
      next: () => this.load(),
      error: () => alert('Obrazac nije obrisan. Pokušaj ponovo.')
    });
  }

  protected download(form: FormTemplateDataOut): void {
    this.client.download(form.documentId).subscribe({
      next: (response) => {
        const url = URL.createObjectURL(response.data);
        const link = document.createElement('a');
        link.href = url;
        link.download = form.fileName ?? response.fileName ?? 'obrazac';
        document.body.appendChild(link);
        link.click();
        link.remove();
        URL.revokeObjectURL(url);
      },
      error: () => alert('Fajl nije preuzet. Pokušaj ponovo.')
    });
  }
}
