import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { AuthService } from '../../../core/auth/auth.service';
import { ProductsFacade } from '../facade/products.facade';
import { Product } from '../../../shared/models/product.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-products-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './products-page.html',
  styleUrls: ['./products-page.scss'],
})
export class ProductsPage implements OnInit, OnDestroy {
  page = 1;
  pageSize = 10;
  searchCtrl = new FormControl<string>('', { nonNullable: true });
  search = '';

  isAdmin = false;

  private destroy$ = new Subject<void>();

  isLoading$!: Observable<boolean>;
  error$!: Observable<string | null>;
  products$!: Observable<Product[]>;
  total$!: Observable<number>;
  totalPages$!: Observable<number>;


  constructor(
    private facade: ProductsFacade,
    private auth: AuthService,
    private router: Router,
    

  ) {
    this.isLoading$ = this.facade.isLoading$;
      this.error$ = this.facade.error$;
      this.products$ = this.facade.products$;
      this.total$ = this.facade.total$;
      this.totalPages$ = this.facade.totalPages$;
  }

  ngOnInit(): void {
    this.isAdmin = this.auth.isAdmin();

    this.searchCtrl.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((value) => {
        this.search = value.trim();
        this.page = 1;
        this.load();
      });

    this.load();
  }

  load(): void {
    this.facade.load({
      page: this.page,
      pageSize: this.pageSize,
      search: this.search || undefined,
    });
  }

  prevPage(): void {
    if (this.page <= 1) return;
    this.page--;
    this.load();
  }

  nextPage(totalPages: number): void {
    if (this.page >= totalPages) return;
    this.page++;
    this.load();
  }

  refresh(): void {
    this.facade.refresh();
  }

  softDelete(id: number): void {
    this.facade.softDelete(id);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
