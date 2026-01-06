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
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./core/ui/layout/layout.component').then((m) => m.LayoutComponent),
    children: [
      { path: 'products',
        loadComponent: () =>
          import('./features/products/products-page/products-page').then((m) => m.ProductsPage),
      },

      { path: 'products/create',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/products/product-form/product-form-page').then((m) => m.ProductFormPage),
      },

      { path: 'products/edit/:id',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/products/product-form/product-form-page').then((m) => m.ProductFormPage),
      },

      { path: 'stock',
        loadComponent: () =>
          import('./features/stock/stock-page/stock-page').then((m) => m.StockPage),
      },

      // ✅ Entry (User y Admin)
      {
        path: 'stock/entry/:productId',
        loadComponent: () =>
          import('./features/stock/stock-movement/stock-movement-page').then(m => m.StockMovementPage),
      },

      // ✅ Exit (User y Admin)
      {
        path: 'stock/exit/:productId',
        loadComponent: () =>
          import('./features/stock/stock-movement/stock-movement-page').then(m => m.StockMovementPage),
      },

      // ✅ Adjustment (SOLO Admin)
      {
        path: 'stock/adjustment/:productId',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () =>
          import('./features/stock/stock-movement/stock-movement-page').then(m => m.StockMovementPage),
      },

      {
      path: 'stock/history',
      loadComponent: () =>
        import('./features/stock/history/stock-history')
          .then(m => m.StockHistoryPage),
    },

      { path: '', redirectTo: 'products', pathMatch: 'full' },
    ],
  },
  


  { path: '**', redirectTo: 'login' },
];

