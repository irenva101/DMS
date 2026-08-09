import { Component, input } from '@angular/core';
import { StepStatus } from '../business-transaction.model';

@Component({
  selector: 'app-status-dot',
  templateUrl: './status-dot.html',
  styleUrl: './status-dot.scss',
  host: {
    '[class]': 'status()',
    '[style.width.px]': 'size()',
    '[style.height.px]': 'size()'
  }
})
export class StatusDot {
  status = input.required<StepStatus>();
  size = input(20);
}
