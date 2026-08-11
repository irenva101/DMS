import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StepStatus } from '../../business-transactions/business-transaction.model';
import { DistributionCaseService } from '../distribution-case.service';
import { DistributionDocumentStatus, DistributionStep } from '../distribution-case.model';

function key(stepName: string, docName: string): string {
  return `${stepName}::${docName}`;
}

@Component({
  selector: 'app-distribution-case-detail',
  imports: [RouterLink],
  templateUrl: './distribution-case-detail.html',
  styleUrl: './distribution-case-detail.scss'
})
export class DistributionCaseDetail {
  private readonly service = inject(DistributionCaseService);

  caseId = input.required<string>();

  protected readonly distributionCase = computed(() => this.service.getCase(this.caseId()));

  protected readonly statuses = signal<Record<string, DistributionDocumentStatus>>({});
  protected readonly files = signal<Record<string, string[]>>({});
  protected readonly dragOverDoc = signal<string | null>(null);
  protected readonly sentToSafe = signal(false);

  constructor() {
    effect(() => {
      const distributionCase = this.distributionCase();
      if (!distributionCase) return;

      const statusMap: Record<string, DistributionDocumentStatus> = {};
      const fileMap: Record<string, string[]> = {};
      for (const step of distributionCase.steps) {
        for (const document of step.documents) {
          const k = key(step.name, document.name);
          statusMap[k] = document.status;
          fileMap[k] = [...document.files];
        }
      }
      this.statuses.set(statusMap);
      this.files.set(fileMap);
      this.sentToSafe.set(distributionCase.sentToSafe);
    });
  }

  protected statusOf(stepName: string, docName: string): DistributionDocumentStatus {
    return this.statuses()[key(stepName, docName)] ?? 'missing';
  }

  protected filesOf(stepName: string, docName: string): string[] {
    return this.files()[key(stepName, docName)] ?? [];
  }

  protected statusLabel(status: DistributionDocumentStatus): string {
    return status === 'submitted' ? 'Predato' : 'Nedostaje';
  }

  protected stepStatusOf(step: DistributionStep): StepStatus {
    const submitted = step.documents.filter((d) => this.statusOf(step.name, d.name) === 'submitted').length;
    if (submitted === 0) return 'missing';
    if (submitted === step.documents.length) return 'verified';
    return 'pending';
  }

  protected stepStatusLabel(status: StepStatus): string {
    return status === 'verified' ? 'Kompletno' : status === 'pending' ? 'U toku' : 'Nedostaje';
  }

  protected isDragOver(stepName: string, docName: string): boolean {
    return this.dragOverDoc() === key(stepName, docName);
  }

  protected onDragOver(event: DragEvent, stepName: string, docName: string): void {
    event.preventDefault();
    this.dragOverDoc.set(key(stepName, docName));
  }

  protected onDragLeave(stepName: string, docName: string): void {
    if (this.isDragOver(stepName, docName)) this.dragOverDoc.set(null);
  }

  protected onDrop(event: DragEvent, stepName: string, docName: string): void {
    event.preventDefault();
    this.dragOverDoc.set(null);
    if (event.dataTransfer?.files.length) this.addFiles(stepName, docName, event.dataTransfer.files);
  }

  protected onFileSelect(event: Event, stepName: string, docName: string): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) this.addFiles(stepName, docName, input.files);
    input.value = '';
  }

  protected removeFile(stepName: string, docName: string, file: string): void {
    const k = key(stepName, docName);
    const current = this.files()[k] ?? [];
    this.files.set({ ...this.files(), [k]: current.filter((f) => f !== file) });
  }

  protected toggleSentToSafe(): void {
    this.sentToSafe.set(!this.sentToSafe());
  }

  private addFiles(stepName: string, docName: string, fileList: FileList): void {
    const k = key(stepName, docName);
    const current = this.files()[k] ?? [];
    const added = Array.from(fileList).map((f) => f.name);
    this.files.set({ ...this.files(), [k]: [...current, ...added] });
    this.statuses.set({ ...this.statuses(), [k]: 'submitted' });
  }
}
