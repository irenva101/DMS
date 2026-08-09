import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pager',
  templateUrl: './pager.html',
  styleUrl: './pager.scss'
})
export class Pager {
  page = input.required<number>();
  totalPages = input.required<number>();
  pageChange = output<number>();

  protected previous(): void {
    if (this.page() > 1) this.pageChange.emit(this.page() - 1);
  }

  protected next(): void {
    if (this.page() < this.totalPages()) this.pageChange.emit(this.page() + 1);
  }
}
