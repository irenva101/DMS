import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StepStatus } from '../../business-transactions/business-transaction.model';
import { MenicaService } from '../menica.service';
import { MenicaDocumentStatus, MenicaStep } from '../menica.model';

function key(stepName: string, docName: string): string {
  return `${stepName}::${docName}`;
}

@Component({
  selector: 'app-menica-detail',
  imports: [RouterLink],
  templateUrl: './menica-detail.html',
  styleUrl: './menica-detail.scss'
})
export class MenicaDetail {
  private readonly service = inject(MenicaService);

  menicaId = input.required<string>();

  protected readonly menica = computed(() => this.service.getMenica(this.menicaId()));

  protected readonly statuses = signal<Record<string, MenicaDocumentStatus>>({});
  protected readonly files = signal<Record<string, string[]>>({});
  protected readonly dragOverDoc = signal<string | null>(null);

  constructor() {
    effect(() => {
      const menica = this.menica();
      if (!menica) return;

      const statusMap: Record<string, MenicaDocumentStatus> = {};
      const fileMap: Record<string, string[]> = {};
      for (const step of menica.steps) {
        for (const document of step.documents) {
          const k = key(step.name, document.name);
          statusMap[k] = document.status;
          fileMap[k] = [...document.files];
        }
      }
      this.statuses.set(statusMap);
      this.files.set(fileMap);
    });
  }

  protected statusOf(stepName: string, docName: string): MenicaDocumentStatus {
    return this.statuses()[key(stepName, docName)] ?? 'missing';
  }

  protected filesOf(stepName: string, docName: string): string[] {
    return this.files()[key(stepName, docName)] ?? [];
  }

  protected statusLabel(status: MenicaDocumentStatus): string {
    return status === 'submitted' ? 'Predato' : 'Nedostaje';
  }

  protected stepStatusOf(step: MenicaStep): StepStatus {
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

  private addFiles(stepName: string, docName: string, fileList: FileList): void {
    const k = key(stepName, docName);
    const current = this.files()[k] ?? [];
    const added = Array.from(fileList).map((f) => f.name);
    this.files.set({ ...this.files(), [k]: [...current, ...added] });
    this.statuses.set({ ...this.statuses(), [k]: 'submitted' });
  }
}
