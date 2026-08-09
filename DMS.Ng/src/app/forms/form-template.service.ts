import { Injectable } from '@angular/core';
import { FormTemplate } from './form-template.model';

@Injectable({ providedIn: 'root' })
export class FormTemplateService {
  private readonly forms: FormTemplate[] = [
    { id: 'OBR-001', name: 'Zahtev za godišnji odmor', category: 'Kadrovski' },
    { id: 'OBR-002', name: 'Rešenje o godišnjem odmoru', category: 'Kadrovski' },
    { id: 'OBR-003', name: 'Nalog za službeni put', category: 'Kadrovski' },
    { id: 'OBR-004', name: 'Ugovor o radu — šablon', category: 'Kadrovski' },
    { id: 'OBR-011', name: 'Ponuda — šablon', category: 'Prodaja' },
    { id: 'OBR-012', name: 'Otpremnica — šablon', category: 'Prodaja' },
    { id: 'OBR-013', name: 'Obrazac za reklamaciju', category: 'Prodaja' },
    { id: 'OBR-021', name: 'Punomoćje', category: 'Opšte' },
    { id: 'OBR-022', name: 'Zapisnik sa sastanka', category: 'Opšte' }
  ];

  getForms(): FormTemplate[] {
    return this.forms;
  }
}
