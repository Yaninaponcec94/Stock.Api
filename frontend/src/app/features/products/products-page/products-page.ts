import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, of } from 'rxjs';
import { switchMap, catchError, finalize, takeUntil, tap } from 'rxjs/operators';
import { ProductsService } from '../../../core/services/products.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { Product } from '../../../shared/models/product.model';
import { PagedResult } from '../../../shared/models/paged-result.model';
@Component({
  selector: 'app-products-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './products-page.html',
  styleUrls: ['./products-page.scss'],
})
export class ProductsPage implements OnInit, OnDestroy {
  isLoading = false;
  error: string | null = null;
  products: Product[] = [];

  page = 1;
  pageSize = 10;
  total = 0;
  totalPages = 1;

  searchCtrl = new FormControl<string>('', { nonNullable: true });
  search = '';

  role = localStorage.getItem('user_role'); 
  isAdmin = (this.role ?? '').toLowerCase() === 'admin';

  
  private destroy$ = new Subject<void>();
  private load$ = new Subject<string>();

  constructor(
    private productsService: ProductsService,
    private cdr: ChangeDetectorRef,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
  this.load$
    .pipe(
      tap((reason) => {
        console.log('[LOAD]', reason, { page: this.page, pageSize: this.pageSize, search: this.search });
        this.isLoading = true;
        this.error = null;
        this.cdr.detectChanges();
      }),
      switchMap(() =>
        this.productsService
          .getProducts({
            page: this.page,
            pageSize: this.pageSize,
            search: this.search || undefined,
          })
          .pipe(
            catchError((err) => {
              console.log('[HTTP ERROR]', err);
              this.error = err?.error?.message ?? err?.message ?? 'Error cargando productos';
              return of({
                items: [],
                totalItems: 0,
                page: this.page,
                pageSize: this.pageSize,
                totalPages: 1,
              } as PagedResult<Product>);
            }),
            finalize(() => {
              this.isLoading = false;
              this.cdr.detectChanges();
            })
          )
      ),
      takeUntil(this.destroy$)
    )
    .subscribe((res) => {
      console.log('[HTTP OK]', res);
      this.products = res.items ?? [];
      this.total = res.totalItems ?? 0;
      this.totalPages = res.totalPages ?? Math.max(1, Math.ceil(this.total / this.pageSize));
      this.page = res.page ?? this.page;

      this.cdr.detectChanges();
    });

  this.searchCtrl.valueChanges
    .pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroy$))
    .subscribe((value) => {
      this.search = value.trim();
      this.page = 1;
      this.load('search');
    });

  this.load('init');
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load(reason = 'manual'): void {
    this.load$.next(reason);
  }

  prevPage(): void {
    if (this.page <= 1 || this.isLoading) return;
    this.page--;
    this.load('prev');
  }

  nextPage(): void {
    if (this.page >= this.totalPages || this.isLoading) return;
    this.page++;
    this.load('next');
  }

  refresh(): void {
    if (this.isLoading) return;
    this.load('refresh');
  }


  softDelete(id: number) {
  if (this.isLoading) return;

  const ok = confirm('¿Desactivar este producto?');
  if (!ok) return;

  this.isLoading = true;
  this.error = null;
  this.cdr.detectChanges();

  this.productsService.softDeleteProduct(id)
    .pipe(finalize(() => { this.isLoading = false; this.cdr.detectChanges(); }))
    .subscribe({
      next: () => this.load('delete'),
      error: (err) => {
        this.error = err?.error?.message ?? err?.message ?? 'Error desactivando producto';
        this.cdr.detectChanges();
      }
    });
}
logout() {
  const ok = confirm('¿Seguro que querés cerrar sesión?');
  if (!ok) return;

  this.auth.logout();
  this.router.navigateByUrl('/login');
}



 
}
