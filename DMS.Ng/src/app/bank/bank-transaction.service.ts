import { Injectable } from '@angular/core';
import { BankDocument, BankDocumentStatus, BankTransaction } from './bank-transaction.model';

function doc(name: string, status: BankDocumentStatus): BankDocument {
  return { name, status, files: status === 'submitted' ? [`${name.replace(/\s+/g, '_')}.pdf`] : [] };
}

function domesticDocuments(
  jci: BankDocumentStatus,
  invoice: BankDocumentStatus,
  order: BankDocumentStatus,
  statement: BankDocumentStatus
): BankDocument[] {
  return [doc('JCI', jci), doc('Faktura', invoice), doc('Nalog 70', order), doc('Izvod iz banke', statement)];
}

function foreignDocuments(
  request: BankDocumentStatus,
  jci: BankDocumentStatus,
  invoice: BankDocumentStatus,
  order: BankDocumentStatus,
  statement: BankDocumentStatus
): BankDocument[] {
  return [
    doc('Zahtev za kupovinu deviza', request),
    doc('JCI', jci),
    doc('Faktura', invoice),
    doc('Nalog 70', order),
    doc('Izvod iz banke', statement)
  ];
}

@Injectable({ providedIn: 'root' })
export class BankTransactionService {
  private readonly transactions: BankTransaction[] = [
    {
      id: 'BNK-001',
      company: 'Metalprom d.o.o.',
      type: 'domestic',
      documents: domesticDocuments('submitted', 'submitted', 'missing', 'missing')
    },
    {
      id: 'BNK-002',
      company: 'Agroplast Novi Sad',
      type: 'domestic',
      documents: domesticDocuments('submitted', 'submitted', 'submitted', 'submitted')
    },
    {
      id: 'BNK-003',
      company: 'BalkanHem Export',
      type: 'domestic',
      documents: domesticDocuments('missing', 'submitted', 'missing', 'missing')
    },
    {
      id: 'BNK-004',
      company: 'Elektrosistem Kragujevac',
      type: 'domestic',
      documents: domesticDocuments('submitted', 'missing', 'missing', 'missing')
    },
    {
      id: 'BNK-005',
      company: 'TransLogistika Beograd',
      type: 'domestic',
      documents: domesticDocuments('submitted', 'submitted', 'submitted', 'submitted')
    },
    {
      id: 'BNK-006',
      company: 'Metalprom d.o.o.',
      type: 'domestic',
      documents: domesticDocuments('missing', 'missing', 'missing', 'missing')
    },
    {
      id: 'BNK-007',
      company: 'BalkanHem Export',
      type: 'domestic',
      documents: domesticDocuments('submitted', 'submitted', 'missing', 'missing')
    },
    {
      id: 'BNK-I-001',
      company: 'Deutsche Handel GmbH',
      type: 'foreign',
      documents: foreignDocuments('submitted', 'submitted', 'submitted', 'missing', 'missing')
    },
    {
      id: 'BNK-I-002',
      company: 'Italmerce S.r.l.',
      type: 'foreign',
      documents: foreignDocuments('submitted', 'submitted', 'submitted', 'submitted', 'submitted')
    },
    {
      id: 'BNK-I-003',
      company: 'Balkan Trade Wien AG',
      type: 'foreign',
      documents: foreignDocuments('missing', 'missing', 'missing', 'missing', 'missing')
    }
  ];

  getTransactions(): BankTransaction[] {
    return this.transactions;
  }

  getTransaction(id: string): BankTransaction | undefined {
    return this.transactions.find((t) => t.id === id);
  }
}
