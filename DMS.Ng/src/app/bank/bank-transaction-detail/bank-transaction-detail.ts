import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BankTransactionService } from '../bank-transaction.service';
import { BankDocumentStatus, BankTransaction } from '../bank-transaction.model';
import { ProgressStep, StepProgress } from '../../shared/step-progress/step-progress';

@Component({
  selector: 'app-bank-transaction-detail',
  imports: [RouterLink, StepProgress],
  templateUrl: './bank-transaction-detail.html',
  styleUrl: './bank-transaction-detail.scss'
})
export class BankTransactionDetail {
  private readonly service = inject(BankTransactionService);

  transactionId = input.required<string>();

  protected readonly transaction = computed(() => this.service.getTransaction(this.transactionId()));

  protected readonly statuses = signal<Record<string, BankDocumentStatus>>({});
  protected readonly files = signal<Record<string, string[]>>({});
  protected readonly dragOverDoc = signal<string | null>(null);

  constructor() {
    effect(() => {
      const tx = this.transaction();
      if (!tx) return;

      const statusMap: Record<string, BankDocumentStatus> = {};
      const fileMap: Record<string, string[]> = {};
      for (const document of tx.documents) {
        statusMap[document.name] = document.status;
        fileMap[document.name] = [...document.files];
      }
      this.statuses.set(statusMap);
      this.files.set(fileMap);
    });
  }

  protected statusOf(name: string): BankDocumentStatus {
    return this.statuses()[name] ?? 'missing';
  }

  protected filesOf(name: string): string[] {
    return this.files()[name] ?? [];
  }

  protected statusLabel(status: BankDocumentStatus): string {
    return status === 'submitted' ? 'Predato' : 'Nedostaje';
  }

  protected progressSteps(tx: BankTransaction): ProgressStep[] {
    return tx.documents.map((document) => ({
      name: document.name,
      status: this.statusOf(document.name) === 'submitted' ? 'verified' : 'missing'
    }));
  }

  protected onDragOver(event: DragEvent, name: string): void {
    event.preventDefault();
    this.dragOverDoc.set(name);
  }

  protected onDragLeave(name: string): void {
    if (this.dragOverDoc() === name) this.dragOverDoc.set(null);
  }

  protected onDrop(event: DragEvent, name: string): void {
    event.preventDefault();
    this.dragOverDoc.set(null);
    if (event.dataTransfer?.files.length) this.addFiles(name, event.dataTransfer.files);
  }

  protected onFileSelect(event: Event, name: string): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) this.addFiles(name, input.files);
    input.value = '';
  }

  protected removeFile(name: string, file: string): void {
    const current = this.files()[name] ?? [];
    this.files.set({ ...this.files(), [name]: current.filter((f) => f !== file) });
  }

  private addFiles(name: string, fileList: FileList): void {
    const current = this.files()[name] ?? [];
    const added = Array.from(fileList).map((f) => f.name);
    this.files.set({ ...this.files(), [name]: [...current, ...added] });
    this.statuses.set({ ...this.statuses(), [name]: 'submitted' });
  }
}
