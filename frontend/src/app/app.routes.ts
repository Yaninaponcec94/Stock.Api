import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
  },

  {
    path: 'products',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/products/products-page/products-page').then(
        (m) => m.ProductsPage
      ),
  },

  {
    path: 'products/create',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () =>
      import('./features/products/product-form/product-form-page').then(
        (m) => m.ProductFormPage
      ),
  },

  {
    path: 'stock',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/stock/stock-page/stock-page').then((m) => m.StockPage),
  },

  {
    path: 'stock/:mode/:productId',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () =>
      import('./features/stock/stock-movement/stock-movement-page').then(
        (m) => m.StockMovementPage
      ),
  },

  {
  path: 'products/edit/:id',
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Admin'] },
  loadComponent: () =>
    import('./features/products/product-form/product-form-page')
      .then(m => m.ProductFormPage),
},

  { path: '**', redirectTo: 'login' },
];
