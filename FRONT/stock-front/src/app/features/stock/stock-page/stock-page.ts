import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize, catchError, of, tap } from 'rxjs';
import { StockService, StockItem } from '../stock.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-stock-page',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './stock-page.html',
  styleUrl: './stock-page.scss',
})
export class StockPage implements OnInit {
  isLoading = false;
  error: string | null = null;

  items: StockItem[] = [];

  constructor(
    private stockService: StockService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
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
        // si el backend no manda isBelowMinStock, lo calculamos
        this.items = (res ?? []).map((x: any) => ({
          ...x,
          isBelowMinStock:
            x.isBelowMinStock ?? (x.currentStock != null && x.minStock != null ? x.currentStock < x.minStock : false),
        }));
        this.cdr.detectChanges();
      });
  }

  refresh() {
    this.load();
  }
}
