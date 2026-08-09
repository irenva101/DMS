export type StepStatus = 'verified' | 'pending' | 'missing';

export interface Step {
  name: string;
  expectedDocument: string;
  status: StepStatus;
  documents: string[];
}

export interface BusinessTransaction {
  client: string;
  code: string;
  steps: Step[];
}
