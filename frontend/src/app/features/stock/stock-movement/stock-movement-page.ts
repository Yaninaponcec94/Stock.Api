import { Component, ChangeDetectorRef, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, finalize, takeUntil } from 'rxjs';
import { StockService } from '../stock.service';

type Mode = 'entry' | 'exit' | 'adjustment';

@Component({
  selector: 'app-stock-movement-page',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './stock-movement-page.html',
})
export class StockMovementPage implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  isLoading = false;
  error: string | null = null;

  productId!: number;
  mode!: Mode;

  form!: FormGroup; 

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private stockService: StockService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      quantity: [null, [Validators.required, Validators.min(1)]],
      reason: [''],
    });
  }

  ngOnInit(): void {
  this.productId = Number(this.route.snapshot.paramMap.get('productId'));
  this.mode = this.route.snapshot.paramMap.get('mode') as any;

  if (!Number.isFinite(this.productId) || this.productId <= 0 || !this.mode) {
    this.error = 'Ruta inválida';
    return;
    }
  }

  submit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.cdr.detectChanges();

    const quantity = Number(this.form.value.quantity);
    const reason = this.form.value.reason?.trim() || undefined;

    const request$ =
      this.mode === 'entry'
        ? this.stockService.entry({ productId: this.productId, quantity, reason })
        : this.mode === 'exit'
        ? this.stockService.exit({ productId: this.productId, quantity, reason })
        : this.stockService.adjustment({ productId: this.productId, quantity, reason });

    request$
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }),
        takeUntil(this.destroy$)
      ).
      subscribe({
        next: () => this.router.navigateByUrl('/stock'),
        error: (err: any) => {
          const detail = err?.error?.detail;
          const title = err?.error?.title;

          this.error =
            detail ||
            err?.error?.message ||
            title ||
            err?.message ||
            'Error aplicando movimiento';

          this.cdr.detectChanges();
        },

      });

  }

  title(): string {
    if (this.mode === 'entry') return 'Entrada de stock';
    if (this.mode === 'exit') return 'Salida de stock';
    return 'Ajuste de stock';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
