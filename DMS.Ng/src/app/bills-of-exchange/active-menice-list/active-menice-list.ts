import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusDot } from '../../business-transactions/status-dot/status-dot';
import { MenicaService } from '../menica.service';
import { Menica, MenicaStep, stepStatus } from '../menica.model';

interface MenicaFlag {
  statusClass: 'missing' | 'pending' | 'done';
  label: string;
}

@Component({
  selector: 'app-active-menice-list',
  imports: [RouterLink, StatusDot],
  templateUrl: './active-menice-list.html',
  styleUrl: './active-menice-list.scss'
})
export class ActiveMeniceList {
  private readonly service = inject(MenicaService);

  protected readonly menice = signal<Menica[]>(this.service.getMenice());

  protected stepStatus(step: MenicaStep) {
    return stepStatus(step);
  }

  protected flag(menica: Menica): MenicaFlag {
    const missing = menica.steps.filter((s) => stepStatus(s) === 'missing').length;
    const pending = menica.steps.filter((s) => stepStatus(s) === 'pending').length;

    if (missing > 0) {
      return { statusClass: 'missing', label: `${missing} ${missing === 1 ? 'korak nedostaje' : 'koraka nedostaje'}` };
    }
    if (pending > 0) {
      return { statusClass: 'pending', label: 'u toku' };
    }
    return { statusClass: 'done', label: 'kompletirano' };
  }
}
