import { Injectable } from '@angular/core';
import { Company, Contract, ContractStatus } from './contract.model';

interface CompanyRecord {
  company: Company;
  contracts: Contract[];
}

@Injectable({ providedIn: 'root' })
export class ContractService {
  private readonly records: CompanyRecord[] = [
    {
      company: { id: 'metalprom', name: 'Metalprom d.o.o.' },
      contracts: [
        { id: 'CTR-0021', title: 'Ugovor o prodaji čelika', signedDate: new Date(2025, 0, 15), expiryDate: new Date(2028, 0, 15), value: 82000 },
        { id: 'CTR-0022', title: 'Okvirni ugovor o saradnji', signedDate: new Date(2023, 2, 3), expiryDate: new Date(2026, 8, 20), value: 15000 }
      ]
    },
    {
      company: { id: 'agroplast-novi-sad', name: 'Agroplast Novi Sad' },
      contracts: [
        { id: 'CTR-0031', title: 'Ugovor o distribuciji', signedDate: new Date(2024, 5, 20), expiryDate: new Date(2027, 5, 20), value: 54000 }
      ]
    },
    {
      company: { id: 'balkanhem-export', name: 'BalkanHem Export' },
      contracts: [
        { id: 'CTR-0041', title: 'Ugovor o izvozu hemikalija', signedDate: new Date(2022, 8, 10), expiryDate: new Date(2025, 8, 10), value: 120000 },
        { id: 'CTR-0042', title: 'Ugovor o skladištenju', signedDate: new Date(2026, 1, 1), expiryDate: new Date(2029, 1, 1), value: 9000 }
      ]
    },
    {
      company: { id: 'elektrosistem-kragujevac', name: 'Elektrosistem Kragujevac' },
      contracts: [
        { id: 'CTR-0051', title: 'Ugovor o zakupu opreme', signedDate: new Date(2024, 4, 5), expiryDate: new Date(2026, 9, 5), value: 30000 }
      ]
    },
    {
      company: { id: 'translogistika-beograd', name: 'TransLogistika Beograd' },
      contracts: [
        { id: 'CTR-0061', title: 'Ugovor o špediciji i transportu', signedDate: new Date(2023, 11, 12), expiryDate: new Date(2026, 11, 12), value: 46000 },
        { id: 'CTR-0062', title: 'Ugovor o zakupu vozila', signedDate: new Date(2022, 0, 1), expiryDate: new Date(2026, 0, 1), value: 21000 }
      ]
    }
  ];

  getCompanies(): Company[] {
    return this.records.map((r) => r.company);
  }

  getCompany(id: string): Company | undefined {
    return this.records.find((r) => r.company.id === id)?.company;
  }

  getContracts(companyId: string): Contract[] {
    return this.records.find((r) => r.company.id === companyId)?.contracts ?? [];
  }

  getContractStatus(contract: Contract): ContractStatus {
    const daysUntilExpiry = (contract.expiryDate.getTime() - Date.now()) / 86_400_000;
    if (daysUntilExpiry < 0) return 'expired';
    if (daysUntilExpiry <= 60) return 'expiring';
    return 'active';
  }

  private readonly recentlyViewedKey = 'dms-contracts-recently-viewed';

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
