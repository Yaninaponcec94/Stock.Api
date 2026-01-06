import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { StockService } from '../../../core/services/stock.service';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stock-history.html',
  styleUrl: './stock-history.scss',
})
export class StockHistoryPage implements OnInit {
  historyItems: any[] = [];
  historyLoading = false;
  historyPage = 1;
  historyPageSize = 10;
  historyTotalItems = 0;
  historyTotalPages = 0;
  historyTypeFilter: 'All' | 'Entry' | 'Exit' | 'Adjustment' = 'All';

  stocksOpen = false;
  constructor(
    private stockService: StockService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory() {
    this.historyLoading = true;

    this.stockService
      .getMovements(undefined, this.historyPage, this.historyPageSize)
      .pipe(
        finalize(() => {
          this.historyLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe((res: any) => {
        this.historyItems = res.items ?? [];
        this.historyTotalItems = res.totalItems ?? 0;
        this.historyTotalPages = res.totalPages ?? 0;
      });
  }

  setHistoryFilter(type: any) {
    this.historyTypeFilter = type;
  }

  filteredHistoryItems() {
    if (this.historyTypeFilter === 'All') return this.historyItems;
    return this.historyItems.filter(x => x.type === this.historyTypeFilter);
  }

  prevHistoryPage() {
    if (this.historyPage > 1) {
      this.historyPage--;
      this.loadHistory();
    }
  }

  nextHistoryPage() {
    if (this.historyPage < this.historyTotalPages) {
      this.historyPage++;
      this.loadHistory();
    }
  }

}
