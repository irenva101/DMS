import { Injectable } from '@angular/core';
import { BankDocument, BankDocumentStatus, BankTransaction } from './bank-transaction.model';

function doc(name: string, status: BankDocumentStatus): BankDocument {
  return { name, status, files: status === 'submitted' ? [`${name.replace(/\s+/g, '_')}.pdf`] : [] };
}

function documents(jci: BankDocumentStatus, invoice: BankDocumentStatus, order: BankDocumentStatus): BankDocument[] {
  return [doc('JCI', jci), doc('Faktura', invoice), doc('Nalog 70', order)];
}

@Injectable({ providedIn: 'root' })
export class BankTransactionService {
  private readonly transactions: BankTransaction[] = [
    { id: 'BNK-001', company: 'Metalprom d.o.o.', documents: documents('submitted', 'submitted', 'missing') },
    { id: 'BNK-002', company: 'Agroplast Novi Sad', documents: documents('submitted', 'submitted', 'submitted') },
    { id: 'BNK-003', company: 'BalkanHem Export', documents: documents('missing', 'submitted', 'missing') },
    { id: 'BNK-004', company: 'Elektrosistem Kragujevac', documents: documents('submitted', 'missing', 'missing') },
    { id: 'BNK-005', company: 'TransLogistika Beograd', documents: documents('submitted', 'submitted', 'submitted') },
    { id: 'BNK-006', company: 'Metalprom d.o.o.', documents: documents('missing', 'missing', 'missing') },
    { id: 'BNK-007', company: 'BalkanHem Export', documents: documents('submitted', 'submitted', 'missing') }
  ];

  getTransactions(): BankTransaction[] {
    return this.transactions;
  }

  getTransaction(id: string): BankTransaction | undefined {
    return this.transactions.find((t) => t.id === id);
  }
}
