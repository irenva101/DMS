import { Injectable } from '@angular/core';
import { BusinessTransaction } from './business-transaction.model';

@Injectable({ providedIn: 'root' })
export class BusinessTransactionService {
  getTransactions(): BusinessTransaction[] {
    return [
      {
        client: 'Metalprom d.o.o.',
        code: 'TRX-2026-0142',
        steps: [
          { name: 'Zahtev za ponudu', expectedDocument: 'Zahtev kupca', status: 'verified', documents: ['Zahtev_kupca.pdf'] },
          { name: 'Ponuda', expectedDocument: 'Ponuda', status: 'verified', documents: ['Ponuda.pdf'] },
          { name: 'Verifikacija', expectedDocument: 'Potvrda ponude', status: 'pending', documents: ['Potvrda_ponude.pdf'] },
          { name: 'Isporuka', expectedDocument: 'Otpremnica', status: 'missing', documents: [] },
          { name: 'Faktura', expectedDocument: 'Faktura', status: 'missing', documents: [] }
        ]
      },
      {
        client: 'Agroplast Novi Sad',
        code: 'TRX-2026-0139',
        steps: [
          { name: 'Zahtev za ponudu', expectedDocument: 'Zahtev kupca', status: 'verified', documents: ['Zahtev_kupca.pdf'] },
          { name: 'Ponuda', expectedDocument: 'Ponuda', status: 'verified', documents: ['Ponuda.pdf'] },
          { name: 'Verifikacija', expectedDocument: 'Potvrda ponude', status: 'verified', documents: ['Potvrda_ponude.pdf'] },
          { name: 'Isporuka', expectedDocument: 'Otpremnica', status: 'pending', documents: ['Otpremnica.pdf'] },
          { name: 'Faktura', expectedDocument: 'Faktura', status: 'missing', documents: [] }
        ]
      },
      {
        client: 'BalkanHem Export',
        code: 'TRX-2026-0147',
        steps: [
          { name: 'Zahtev za ponudu', expectedDocument: 'Zahtev kupca', status: 'verified', documents: ['Zahtev_kupca.pdf'] },
          { name: 'Ponuda', expectedDocument: 'Ponuda', status: 'missing', documents: [] },
          { name: 'Verifikacija', expectedDocument: 'Potvrda ponude', status: 'missing', documents: [] },
          { name: 'Isporuka', expectedDocument: 'Otpremnica', status: 'missing', documents: [] },
          { name: 'Faktura', expectedDocument: 'Faktura', status: 'missing', documents: [] }
        ]
      },
      {
        client: 'Elektrosistem Kragujevac',
        code: 'TRX-2026-0151',
        steps: [
          { name: 'Zahtev za ponudu', expectedDocument: 'Zahtev kupca', status: 'pending', documents: ['Zahtev_kupca.pdf'] },
          { name: 'Ponuda', expectedDocument: 'Ponuda', status: 'missing', documents: [] },
          { name: 'Verifikacija', expectedDocument: 'Potvrda ponude', status: 'missing', documents: [] },
          { name: 'Isporuka', expectedDocument: 'Otpremnica', status: 'missing', documents: [] },
          { name: 'Faktura', expectedDocument: 'Faktura', status: 'missing', documents: [] }
        ]
      },
      {
        client: 'TransLogistika Beograd',
        code: 'TRX-2026-0130',
        steps: [
          { name: 'Zahtev za ponudu', expectedDocument: 'Zahtev kupca', status: 'verified', documents: ['Zahtev_kupca.pdf'] },
          { name: 'Ponuda', expectedDocument: 'Ponuda', status: 'verified', documents: ['Ponuda.pdf'] },
          { name: 'Verifikacija', expectedDocument: 'Potvrda ponude', status: 'verified', documents: ['Potvrda_ponude.pdf'] },
          { name: 'Isporuka', expectedDocument: 'Otpremnica', status: 'verified', documents: ['Otpremnica.pdf'] },
          { name: 'Faktura', expectedDocument: 'Faktura', status: 'verified', documents: ['Faktura.pdf'] }
        ]
      }
    ];
  }

  getTransaction(code: string): BusinessTransaction | undefined {
    return this.getTransactions().find((t) => t.code === code);
  }
}
