import { Component, ChangeDetectorRef, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, finalize, takeUntil, Observable } from 'rxjs';
import { ProductsService} from '../../../core/services/products.service';
import { Product } from '../../../shared/models/product.model';
import { CreateProductDto } from '../../products/models/create-product.dto';
import { UpdateProductDto } from '../../products/models/update-product.dto';

@Component({
  selector: 'app-product-form-page',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './product-form-page.html',
  styleUrls: ['./product-form-page.scss'],
})
export class ProductFormPage implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  isLoading = false;
  error: string | null = null;

  productId: number | null = null;
  isEditMode = false;

  form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private productsService: ProductsService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      minStock: [1, [Validators.required, Validators.min(1), Validators.max(100000)]],
    });

  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const parsedId = idParam ? Number(idParam) : NaN;

    this.productId = Number.isFinite(parsedId) ? parsedId : null;
    this.isEditMode = this.productId !== null;

    if (this.isEditMode && this.productId) {
      this.loadProduct(this.productId);
    }
  }

  private loadProduct(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.cdr.detectChanges();

    this.productsService
      .getProductById(id)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: (p: Product) => {
          this.form.patchValue({
            name: p.name,
            minStock: p.minStock,
          });
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.error = err?.error?.message ?? err?.message ?? 'Error cargando producto';
          this.cdr.detectChanges();
        },
      });
  }

  submit(): void {
    console.log('[PRODUCT FORM] submit()', { isEditMode: this.isEditMode, productId: this.productId });

    if (this.form.invalid || this.isLoading) {
      console.log('[PRODUCT FORM] invalid form', this.form.value);
      this.form.markAllAsTouched();
      return;
    }

    const name = String(this.form.value['name'] ?? '').trim();
    const minStock = Number(this.form.value['minStock'] ?? 0);

    const createDto: CreateProductDto = { name, minStock };

    this.isLoading = true;
    this.error = null;
    this.cdr.detectChanges();

    let request$: Observable<unknown>;

    if (this.isEditMode && this.productId) {
      const updateDto: UpdateProductDto = {
        name,
        minStock,
        isActive: true, 
      };
      request$ = this.productsService.updateProduct(this.productId, updateDto);
    } else {
      request$ = this.productsService.createProduct(createDto);
    }

    request$
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
          console.log('[PRODUCT FORM] finalize');
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: () => {
          console.log('[PRODUCT FORM] OK');
          this.router.navigateByUrl('/products');
        },
        error: (err: any) => {
          console.log('[PRODUCT FORM] ERROR', err);
          this.error = err?.error?.message ?? err?.message ?? 'Error guardando producto';
          this.cdr.detectChanges();
        },
      });
  }

  cancel(): void {
    this.router.navigateByUrl('/products');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}


