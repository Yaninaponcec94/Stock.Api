import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../auth/auth.service'; 
import { NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-layout',
imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],

  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
  isAdmin = false;

  productsOpen = false;
  isProductsSectionActive = false;

  stocksOpen = false;


  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.getIsAdminSafe();
    this.updateProductsActive(this.router.url);

    this.router.events
   
  .pipe(filter(e => e instanceof NavigationEnd))
  .subscribe(() => {
    this.isAdmin = this.getIsAdminSafe();
    this.updateProductsActive(this.router.url);
    if (this.isProductsSectionActive) this.productsOpen = true;
  });
  }

  toggleProductsMenu(): void {
  this.productsOpen = !this.productsOpen;
}

private updateProductsActive(url: string): void {
  this.isProductsSectionActive =
    url.startsWith('/products') || url.startsWith('/products/');
}

  

  private getIsAdminSafe(): boolean {
    const anyAuth: any = this.auth;
    if (typeof anyAuth.isAdmin === 'function') return !!anyAuth.isAdmin();
    if (typeof anyAuth.hasRole === 'function') return !!anyAuth.hasRole('Admin');
    if (typeof anyAuth.getRole === 'function') return anyAuth.getRole() === 'Admin';

    const role =
      localStorage.getItem('role') ||
      localStorage.getItem('userRole') ||
      localStorage.getItem('roles');

    if (!role) return false;
    try {
      const parsed = JSON.parse(role);
      if (Array.isArray(parsed)) return parsed.includes('Admin');
    } catch {}

    return role === 'Admin';
  }

    toggleStocksMenu(): void {
      this.stocksOpen = !this.stocksOpen;
  }

  logout(): void {
  const ok = confirm('¿Seguro que querés cerrar sesión?');

  if (!ok) return;

  this.auth.logout();
  this.router.navigateByUrl('/login');
}

}
