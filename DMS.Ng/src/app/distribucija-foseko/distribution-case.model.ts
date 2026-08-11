import { StepStatus } from '../business-transactions/business-transaction.model';

export type DistributionCustomerType = 'domestic' | 'foreign';
export type DistributionDocumentStatus = 'submitted' | 'missing';

export interface DistributionDocument {
  name: string;
  status: DistributionDocumentStatus;
  files: string[];
}

export interface DistributionStep {
  name: string;
  documents: DistributionDocument[];
}

export interface DistributionCase {
  id: string;
  company: string;
  customerType: DistributionCustomerType;
  steps: DistributionStep[];
  sentToSafe: boolean;
}

export function stepStatus(step: DistributionStep): StepStatus {
  const submitted = step.documents.filter((d) => d.status === 'submitted').length;
  if (submitted === 0) return 'missing';
  if (submitted === step.documents.length) return 'verified';
  return 'pending';
}
