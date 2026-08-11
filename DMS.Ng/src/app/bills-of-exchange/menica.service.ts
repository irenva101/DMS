import { Injectable } from '@angular/core';
import { Menica, MenicaDocument, MenicaDocumentStatus, MenicaStep } from './menica.model';

const STEP_NAMES = [
  'Zahtev za kupovinu deviza',
  'Dostava menice od strane dužnika',
  'Dostava menice u banku',
  'Izvod iz banke'
];

const STEP_DOCUMENTS: string[][] = [
  ['Zahtev za kupovinu deviza'],
  ['Dostava menice', 'Menično ovlašćenje', 'Naplata menice'],
  ['Dostava menice', 'Nalog za isplatu'],
  ['Izvod iz banke']
];

function doc(name: string, status: MenicaDocumentStatus): MenicaDocument {
  return { name, status, files: status === 'submitted' ? [`${name.replace(/\s+/g, '_')}.pdf`] : [] };
}

function steps(statuses: MenicaDocumentStatus[][]): MenicaStep[] {
  return STEP_NAMES.map((name, i) => ({
    name,
    documents: STEP_DOCUMENTS[i].map((docName, j) => doc(docName, statuses[i][j]))
  }));
}

@Injectable({ providedIn: 'root' })
export class MenicaService {
  private readonly menice: Menica[] = [
    {
      id: 'MNC-001',
      company: 'Metalprom d.o.o.',
      steps: steps([
        ['submitted'],
        ['submitted', 'submitted', 'missing'],
        ['missing', 'missing'],
        ['missing']
      ])
    },
    {
      id: 'MNC-002',
      company: 'Agroplast Novi Sad',
      steps: steps([
        ['submitted'],
        ['submitted', 'submitted', 'submitted'],
        ['submitted', 'submitted'],
        ['submitted']
      ])
    },
    {
      id: 'MNC-003',
      company: 'BalkanHem Export',
      steps: steps([
        ['submitted'],
        ['missing', 'missing', 'missing'],
        ['missing', 'missing'],
        ['missing']
      ])
    },
    {
      id: 'MNC-004',
      company: 'Elektrosistem Kragujevac',
      steps: steps([
        ['submitted'],
        ['submitted', 'missing', 'missing'],
        ['missing', 'missing'],
        ['missing']
      ])
    }
  ];

  getMenice(): Menica[] {
    return this.menice;
  }

  getMenica(id: string): Menica | undefined {
    return this.menice.find((m) => m.id === id);
  }
}
