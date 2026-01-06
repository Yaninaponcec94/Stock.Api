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

}
