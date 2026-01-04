import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize, switchMap, tap } from 'rxjs/operators';
import { ProductsService } from '../../../core/services/products.service';
import { DialogService } from '../../../core/ui/dialog.service';
import { Product } from '../../../shared/models/product.model';
import { PagedResult } from '../../../shared/models/paged-result.model';

export interface ProductsQuery {
  page: number;
  pageSize: number;
  search?: string;
}

@Injectable({ providedIn: 'root' })
export class ProductsFacade {
  private readonly _isLoading = new BehaviorSubject<boolean>(false);
  readonly isLoading$ = this._isLoading.asObservable();

  private readonly _error = new BehaviorSubject<string | null>(null);
  readonly error$ = this._error.asObservable();

  private readonly _products = new BehaviorSubject<Product[]>([]);
  readonly products$ = this._products.asObservable();

  private readonly _total = new BehaviorSubject<number>(0);
  readonly total$ = this._total.asObservable();

  private readonly _totalPages = new BehaviorSubject<number>(1);
  readonly totalPages$ = this._totalPages.asObservable();

  private lastQuery: ProductsQuery | null = null;

  constructor(
    private api: ProductsService,
    private dialog: DialogService
  ) {}

  load(query: ProductsQuery): void {
    this.lastQuery = query;
    this._isLoading.next(true);
    this._error.next(null);

    this.api.getProducts(query).pipe(
      tap((res: PagedResult<Product>) => {
        const items = res.items ?? [];
        const totalItems = res.totalItems ?? 0;
        const totalPages = res.totalPages ?? Math.max(1, Math.ceil(totalItems / query.pageSize));

        this._products.next(items);
        this._total.next(totalItems);
        this._totalPages.next(totalPages);
      }),
      catchError((err) => {
        const msg = err?.error?.message ?? err?.message ?? 'Error cargando productos';
        this._error.next(msg);

        this._products.next([]);
        this._total.next(0);
        this._totalPages.next(1);

        return of(null);
      }),
      finalize(() => this._isLoading.next(false))
    ).subscribe();
  }

  refresh(): void {
    if (!this.lastQuery) return;
    this.load(this.lastQuery);
  }

  softDelete(id: number): void {
    this.dialog.confirm('¿Desactivar este producto?').pipe(
      switchMap((ok) => {
        if (!ok) return of(null);

        this._isLoading.next(true);
        this._error.next(null);

        return this.api.softDeleteProduct(id).pipe(
          tap(() => this.refresh()),
          catchError((err) => {
            const msg = err?.error?.message ?? err?.message ?? 'Error desactivando producto';
            this._error.next(msg);
            return of(null);
          }),
          finalize(() => this._isLoading.next(false))
        );
      })
    ).subscribe();
  }
}
