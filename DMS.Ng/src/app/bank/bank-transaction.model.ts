export type BankDocumentStatus = 'submitted' | 'missing';

export interface BankDocument {
  name: string;
  status: BankDocumentStatus;
  files: string[];
}

export interface BankTransaction {
  id: string;
  company: string;
  documents: BankDocument[];
}
