import { Component, computed, input } from '@angular/core';
import { StepStatus } from '../../business-transactions/business-transaction.model';

export interface ProgressStep {
  name: string;
  status: StepStatus;
}

@Component({
  selector: 'app-step-progress',
  templateUrl: './step-progress.html',
  styleUrl: './step-progress.scss'
})
export class StepProgress {
  steps = input.required<ProgressStep[]>();

  protected readonly completedCount = computed(() => this.steps().filter((s) => s.status === 'verified').length);

  protected readonly currentStepName = computed(() => this.steps().find((s) => s.status !== 'verified')?.name ?? null);
}
