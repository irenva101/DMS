export type BillStatus = 'active' | 'due-soon' | 'due';

export interface Bill {
  id: string;
  amount: number;
  issueDate: Date;
  maturityDate: Date;
}

export interface Company {
  id: string;
  name: string;
}
