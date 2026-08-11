import { StepStatus } from '../business-transactions/business-transaction.model';

export type MenicaDocumentStatus = 'submitted' | 'missing';

export interface MenicaDocument {
  name: string;
  status: MenicaDocumentStatus;
  files: string[];
}

export interface MenicaStep {
  name: string;
  documents: MenicaDocument[];
}

export interface Menica {
  id: string;
  company: string;
  steps: MenicaStep[];
}

export function stepStatus(step: MenicaStep): StepStatus {
  const submitted = step.documents.filter((d) => d.status === 'submitted').length;
  if (submitted === 0) return 'missing';
  if (submitted === step.documents.length) return 'verified';
  return 'pending';
}
