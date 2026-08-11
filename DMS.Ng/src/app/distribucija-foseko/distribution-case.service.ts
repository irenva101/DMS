import { Injectable } from '@angular/core';
import { DistributionCase, DistributionDocument, DistributionDocumentStatus, DistributionStep } from './distribution-case.model';

const CORE_STEP_NAMES = [
  'Zahtev za ponudu',
  'Dobijena ponuda',
  'Prihvatanje ponude od Foseka',
  'Ponuda za kupca',
  'Prihvatanje ponude kupca'
];

const CORE_STEP_DOCUMENTS: string[][] = [
  ['Zahtev za ponudu'],
  ['Dobijena ponuda'],
  ['Narudžbina po njihovoj ponudi', 'Dokument o prihvatanju ponude'],
  ['Ponuda za kupca'],
  ['Prihvatanje ponude', 'Dokument narudžbe']
];

const DOMESTIC_EXTRA_STEP_NAMES = ['Otpremnica', 'Faktura'];
const DOMESTIC_EXTRA_STEP_DOCUMENTS: string[][] = [['Otpremnica'], ['Faktura']];

const FOREIGN_EXTRA_STEP_NAMES = ['Isporuka'];
const FOREIGN_EXTRA_STEP_DOCUMENTS: string[][] = [
  ['JCI', 'Potvrda o kvalitetu', 'Račun', 'CMR', 'Izjava o poreklu na računu']
];

function doc(name: string, status: DistributionDocumentStatus): DistributionDocument {
  return { name, status, files: status === 'submitted' ? [`${name.replace(/\s+/g, '_')}.pdf`] : [] };
}

function buildSteps(names: string[], docNames: string[][], statuses: DistributionDocumentStatus[][]): DistributionStep[] {
  return names.map((name, i) => ({
    name,
    documents: docNames[i].map((docName, j) => doc(docName, statuses[i][j]))
  }));
}

function foreignSteps(statuses: DistributionDocumentStatus[][]): DistributionStep[] {
  return buildSteps(
    [...CORE_STEP_NAMES, ...FOREIGN_EXTRA_STEP_NAMES],
    [...CORE_STEP_DOCUMENTS, ...FOREIGN_EXTRA_STEP_DOCUMENTS],
    statuses
  );
}

function domesticSteps(statuses: DistributionDocumentStatus[][]): DistributionStep[] {
  return buildSteps(
    [...CORE_STEP_NAMES, ...DOMESTIC_EXTRA_STEP_NAMES],
    [...CORE_STEP_DOCUMENTS, ...DOMESTIC_EXTRA_STEP_DOCUMENTS],
    statuses
  );
}

@Injectable({ providedIn: 'root' })
export class DistributionCaseService {
  private readonly cases: DistributionCase[] = [
    {
      id: 'DIST-D-001',
      company: 'Metalprom d.o.o.',
      customerType: 'domestic',
      sentToSafe: true,
      steps: domesticSteps([
        ['submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['submitted'],
        ['submitted']
      ])
    },
    {
      id: 'DIST-D-002',
      company: 'Agroplast Novi Sad',
      customerType: 'domestic',
      sentToSafe: false,
      steps: domesticSteps([
        ['submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['missing'],
        ['missing']
      ])
    },
    {
      id: 'DIST-D-003',
      company: 'BalkanHem Export',
      customerType: 'domestic',
      sentToSafe: false,
      steps: domesticSteps([
        ['submitted'],
        ['missing'],
        ['missing', 'missing'],
        ['missing'],
        ['missing', 'missing'],
        ['missing'],
        ['missing']
      ])
    },
    {
      id: 'DIST-I-001',
      company: 'Alpin Chemie AG',
      customerType: 'foreign',
      sentToSafe: false,
      steps: foreignSteps([
        ['submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['submitted'],
        ['submitted', 'submitted'],
        ['submitted', 'submitted', 'submitted', 'submitted', 'submitted']
      ])
    },
    {
      id: 'DIST-I-002',
      company: 'Nordic Foundry Supplies',
      customerType: 'foreign',
      sentToSafe: false,
      steps: foreignSteps([
        ['submitted'],
        ['submitted'],
        ['submitted', 'missing'],
        ['missing'],
        ['missing', 'missing'],
        ['missing', 'missing', 'missing', 'missing', 'missing']
      ])
    },
    {
      id: 'DIST-I-003',
      company: 'Adriatic Metal Trade',
      customerType: 'foreign',
      sentToSafe: false,
      steps: foreignSteps([
        ['submitted'],
        ['missing'],
        ['missing', 'missing'],
        ['missing'],
        ['missing', 'missing'],
        ['missing', 'missing', 'missing', 'missing', 'missing']
      ])
    }
  ];

  getCases(): DistributionCase[] {
    return this.cases;
  }

  getCase(id: string): DistributionCase | undefined {
    return this.cases.find((c) => c.id === id);
  }
}
