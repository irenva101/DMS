import { Injectable } from '@angular/core';
import { Bill, BillStatus, Company } from './bill.model';

interface CompanyRecord {
  company: Company;
  bills: Bill[];
}

@Injectable({ providedIn: 'root' })
export class BillService {
  private readonly records: CompanyRecord[] = [
    {
      company: { id: 'metalprom', name: 'Metalprom d.o.o.' },
      bills: [
        { id: 'MEN-0011', amount: 850000, issueDate: new Date(2026, 1, 10), maturityDate: new Date(2026, 7, 20) },
        { id: 'MEN-0012', amount: 320000, issueDate: new Date(2026, 0, 5), maturityDate: new Date(2026, 6, 5) }
      ]
    },
    {
      company: { id: 'agroplast-novi-sad', name: 'Agroplast Novi Sad' },
      bills: [
        { id: 'MEN-0021', amount: 1250000, issueDate: new Date(2026, 5, 1), maturityDate: new Date(2026, 11, 1) }
      ]
    },
    {
      company: { id: 'balkanhem-export', name: 'BalkanHem Export' },
      bills: [
        { id: 'MEN-0031', amount: 2100000, issueDate: new Date(2026, 0, 12), maturityDate: new Date(2026, 6, 12) },
        { id: 'MEN-0032', amount: 480000, issueDate: new Date(2026, 6, 20), maturityDate: new Date(2026, 7, 15) }
      ]
    },
    {
      company: { id: 'elektrosistem-kragujevac', name: 'Elektrosistem Kragujevac' },
      bills: [
        { id: 'MEN-0041', amount: 690000, issueDate: new Date(2026, 2, 1), maturityDate: new Date(2026, 10, 1) }
      ]
    },
    {
      company: { id: 'translogistika-beograd', name: 'TransLogistika Beograd' },
      bills: [
        { id: 'MEN-0051', amount: 410000, issueDate: new Date(2026, 4, 15), maturityDate: new Date(2026, 10, 15) },
        { id: 'MEN-0052', amount: 275000, issueDate: new Date(2026, 1, 1), maturityDate: new Date(2026, 2, 1) }
      ]
    }
  ];

  getCompanies(): Company[] {
    return this.records.map((r) => r.company);
  }

  getCompany(id: string): Company | undefined {
    return this.records.find((r) => r.company.id === id)?.company;
  }

  getBills(companyId: string): Bill[] {
    return this.records.find((r) => r.company.id === companyId)?.bills ?? [];
  }

  getBillStatus(bill: Bill): BillStatus {
    const daysUntilMaturity = (bill.maturityDate.getTime() - Date.now()) / 86_400_000;
    if (daysUntilMaturity < 0) return 'due';
    if (daysUntilMaturity <= 14) return 'due-soon';
    return 'active';
  }

  private readonly recentlyViewedKey = 'dms-bills-recently-viewed';

  recordView(companyId: string): void {
    const ids = this.readRecentlyViewedIds().filter((id) => id !== companyId);
    ids.unshift(companyId);
    localStorage.setItem(this.recentlyViewedKey, JSON.stringify(ids.slice(0, 5)));
  }

  getRecentlyViewed(): Company[] {
    return this.readRecentlyViewedIds()
      .map((id) => this.getCompany(id))
      .filter((company): company is Company => !!company);
  }

  private readRecentlyViewedIds(): string[] {
    try {
      const raw = localStorage.getItem(this.recentlyViewedKey);
      return raw ? (JSON.parse(raw) as string[]) : [];
    } catch {
      return [];
    }
  }
}
