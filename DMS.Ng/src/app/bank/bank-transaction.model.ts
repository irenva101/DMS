export type BankDocumentStatus = 'submitted' | 'missing';
export type BankTransactionType = 'domestic' | 'foreign';

export interface BankDocument {
  name: string;
  status: BankDocumentStatus;
  files: string[];
}

export interface BankTransaction {
  id: string;
  company: string;
  type: BankTransactionType;
  documents: BankDocument[];
}
