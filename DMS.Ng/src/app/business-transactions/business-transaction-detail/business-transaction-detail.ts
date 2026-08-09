import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusDot } from '../status-dot/status-dot';
import { BusinessTransactionService } from '../business-transaction.service';
import { StepStatus } from '../business-transaction.model';

@Component({
  selector: 'app-business-transaction-detail',
  imports: [RouterLink, StatusDot],
  templateUrl: './business-transaction-detail.html',
  styleUrl: './business-transaction-detail.scss'
})
export class BusinessTransactionDetail {
  private readonly service = inject(BusinessTransactionService);

  code = input.required<string>();

  protected readonly transaction = computed(() => this.service.getTransaction(this.code()));

  protected readonly statuses = signal<Record<string, StepStatus>>({});
  protected readonly documents = signal<Record<string, string[]>>({});
  protected readonly dragOverStep = signal<string | null>(null);

  constructor() {
    effect(() => {
      const tx = this.transaction();
      if (!tx) return;

      const statusMap: Record<string, StepStatus> = {};
      const documentMap: Record<string, string[]> = {};
      for (const step of tx.steps) {
        statusMap[step.name] = step.status;
        documentMap[step.name] = [...step.documents];
      }
      this.statuses.set(statusMap);
      this.documents.set(documentMap);
    });
  }

  protected statusOf(stepName: string): StepStatus {
    return this.statuses()[stepName] ?? 'missing';
  }

  protected documentsOf(stepName: string): string[] {
    return this.documents()[stepName] ?? [];
  }

  protected statusLabel(status: StepStatus): string {
    return status === 'verified' ? 'Verifikovano' : status === 'pending' ? 'Priloženo' : 'Nedostaje';
  }

  protected onDragOver(event: DragEvent, stepName: string): void {
    event.preventDefault();
    this.dragOverStep.set(stepName);
  }

  protected onDragLeave(stepName: string): void {
    if (this.dragOverStep() === stepName) this.dragOverStep.set(null);
  }

  protected onDrop(event: DragEvent, stepName: string): void {
    event.preventDefault();
    this.dragOverStep.set(null);
    if (event.dataTransfer?.files.length) this.addDocuments(stepName, event.dataTransfer.files);
  }

  protected onFileSelect(event: Event, stepName: string): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) this.addDocuments(stepName, input.files);
    input.value = '';
  }

  protected removeDocument(stepName: string, document: string): void {
    const current = this.documents()[stepName] ?? [];
    this.documents.set({ ...this.documents(), [stepName]: current.filter((d) => d !== document) });
  }

  protected verify(stepName: string): void {
    this.statuses.set({ ...this.statuses(), [stepName]: 'verified' });
  }

  private addDocuments(stepName: string, files: FileList): void {
    const current = this.documents()[stepName] ?? [];
    const added = Array.from(files).map((f) => f.name);
    this.documents.set({ ...this.documents(), [stepName]: [...current, ...added] });

    if (this.statusOf(stepName) === 'missing') {
      this.statuses.set({ ...this.statuses(), [stepName]: 'pending' });
    }
  }
}
