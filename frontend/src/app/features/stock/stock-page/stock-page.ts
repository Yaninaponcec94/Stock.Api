import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize, catchError, of, tap } from 'rxjs';
import { StockService } from '../../../core/services/stock.service';
import { RouterModule } from '@angular/router';
import { StockItem } from '../../../shared/models/stock-item.model';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-stock-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './stock-page.html',
  styleUrl: './stock-page.scss',
})
export class StockPage implements OnInit {
  isLoading = false;
  error: string | null = null;

  items: StockItem[] = [];
  isAdmin = false;

  showHistory = false;
  historyItems: any[] = [];
  historyLoading = false;
  historyPage = 1;
  historyPageSize = 10;

  selectedProductId?: number;
  selectedProductName = '';

  historyTotalItems = 0;
  historyTotalPages = 0;
  historyTypeFilter: 'All' | 'Entry' | 'Exit' | 'Adjustment' = 'All';

  constructor(
    private stockService: StockService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = (this.authService.getRole() ?? '').toLowerCase() === 'admin';
    this.load();
  }

  load() {
    console.log('[STOCK] load');
    this.isLoading = true;
    this.error = null;
    this.cdr.detectChanges();

    this.stockService
      .getStock()
      .pipe(
        tap((res) => console.log('[STOCK OK]', res)),
        catchError((err) => {
          console.log('[STOCK ERROR]', err);
          this.error = err?.error?.message ?? 'Error cargando stock';
          return of([] as StockItem[]);
        }),
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe((res) => {
        console.log('PRIMER ITEM STOCK RAW:', res?.[0]);

        this.items = (res ?? []).map((x: any) => ({
          ...x,
          isBelowMinStock:
            x.isBelowMinStock ??
            (x.quantity != null && x.minStock != null ? x.quantity < x.minStock : false),
        }));

        console.log('PRIMER ITEM STOCK MAPEADO:', this.items?.[0]);
        this.cdr.detectChanges();
      });
  }

  refresh() {
    this.load();
  }

loadHistory(productId?: number) {
  console.log('CLICK HISTORIAL PARAM=', productId);

  this.showHistory = true;
  this.historyLoading = true;
  this.selectedProductId = productId;

  this.selectedProductName = productId
    ? (this.items.find(i => i.productId === productId)?.productName ?? '')
    : '';

  this.stockService
    .getMovements(productId, this.historyPage, this.historyPageSize)
    .pipe(
      catchError(err => {
        console.error('[HISTORY ERROR]', err);
        
        this.historyItems = [];
        this.historyTotalItems = 0;
        this.historyTotalPages = 0;
        return of({ items: [], totalItems: 0, totalPages: 0 });
      }),
      finalize(() => {
        this.historyLoading = false;
        this.cdr.detectChanges();
      })
    )
    .subscribe((res: any) => {
      this.historyItems = res?.items ?? [];
      this.historyTotalItems = res?.totalItems ?? 0;
      this.historyTotalPages = res?.totalPages ?? 0;

      setTimeout(() => {
        document
          .getElementById('history-section')
          ?.scrollIntoView({ behavior: 'smooth' });
      }, 0);
    });
}


prevHistoryPage() {
  if (this.historyPage <= 1) return;
  this.historyPage--;
  this.loadHistory(this.selectedProductId);
}

nextHistoryPage() {
  if (this.historyPage >= (this.historyTotalPages || 1)) return;
  this.historyPage++;
  this.loadHistory(this.selectedProductId);
}


  setHistoryFilter(type: 'All' | 'Entry' | 'Exit' | 'Adjustment') {
    this.historyTypeFilter = type;
  
  }

  filteredHistoryItems() {
    if (this.historyTypeFilter === 'All') return this.historyItems;
    return (this.historyItems ?? []).filter((x) => x.type === this.historyTypeFilter);
  }
}
