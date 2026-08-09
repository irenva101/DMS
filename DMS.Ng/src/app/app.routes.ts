import { Routes } from '@angular/router';

function loadPlaceholder() {
  return import('./shared/module-placeholder/module-placeholder').then((m) => m.ModulePlaceholder);
}

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'business-transactions' },
  {
    path: 'business-transactions',
    loadComponent: () =>
      import('./business-transactions/business-transaction-list/business-transaction-list').then(
        (m) => m.BusinessTransactionList
      )
  },
  {
    path: 'business-transactions/:code',
    loadComponent: () =>
      import('./business-transactions/business-transaction-detail/business-transaction-detail').then(
        (m) => m.BusinessTransactionDetail
      )
  },
  { path: 'import', loadComponent: loadPlaceholder, data: { name: 'Uvoz' } },
  { path: 'export', loadComponent: loadPlaceholder, data: { name: 'Izvoz' } },
  {
    path: 'contracts',
    loadComponent: () => import('./contracts/company-list/company-list').then((m) => m.CompanyList)
  },
  {
    path: 'contracts/:companyId',
    loadComponent: () => import('./contracts/company-contracts/company-contracts').then((m) => m.CompanyContracts)
  },
  {
    path: 'bank',
    loadComponent: () => import('./bank/bank-transaction-list/bank-transaction-list').then((m) => m.BankTransactionList)
  },
  {
    path: 'bank/:transactionId',
    loadComponent: () =>
      import('./bank/bank-transaction-detail/bank-transaction-detail').then((m) => m.BankTransactionDetail)
  },
  {
    path: 'bills-of-exchange',
    loadComponent: () => import('./bills-of-exchange/company-list/company-list').then((m) => m.CompanyList)
  },
  {
    path: 'bills-of-exchange/:companyId',
    loadComponent: () => import('./bills-of-exchange/company-bills/company-bills').then((m) => m.CompanyBills)
  },
  {
    path: 'forms',
    loadComponent: () => import('./forms/form-list/form-list').then((m) => m.FormList)
  },
  { path: 'warehouse', loadComponent: loadPlaceholder, data: { name: 'Skladište' } },
  { path: 'archive', loadComponent: loadPlaceholder, data: { name: 'Arhiva' } }
];
