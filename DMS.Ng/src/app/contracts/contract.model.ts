export type ContractStatus = 'active' | 'expiring' | 'expired';

export interface Contract {
  id: string;
  title: string;
  signedDate: Date;
  expiryDate: Date;
  value: number;
}

export interface Company {
  id: string;
  name: string;
}
