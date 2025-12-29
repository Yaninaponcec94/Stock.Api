import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { catchError, finalize, of, tap } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  isLoading = false;
  apiError: string | null = null;

  form;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  
  ) {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  submit() {
    console.log('[LOGIN] submit llamado');

    if (this.form.invalid || this.isLoading) {
      console.log('[LOGIN] form inválido o loading');
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue();
    console.log('[LOGIN] payload', payload);

    this.apiError = null;
    this.isLoading = true;
    this.cdr.detectChanges();

    this.auth
      .login(payload as any)
      .pipe(
        tap((res) => {
          console.log('[LOGIN OK]', res);
        }),
        catchError((err) => {
          console.log('[LOGIN ERROR]', err);
          this.apiError = err?.error?.message ?? 'Error al iniciar sesión';
          return of(null);
        }),
        finalize(() => {
          console.log('[LOGIN FINALIZE]');
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe((res) => {
        console.log('[LOGIN SUBSCRIBE]', res);

        if (!res) return;

        console.log('[LOGIN] navegando a /products');
        this.router.navigateByUrl('/products');
        this.cdr.detectChanges();
      });
  }
  
  logout() {
  this.auth.logout();
  this.router.navigateByUrl('/login');
}

}
