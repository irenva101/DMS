import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-module-placeholder',
  templateUrl: './module-placeholder.html'
})
export class ModulePlaceholder {
  protected readonly name = inject(ActivatedRoute).snapshot.data['name'];
}
