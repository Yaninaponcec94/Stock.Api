import { CanActivateFn, ActivatedRouteSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const allowedRoles = (route.data?.['roles'] as string[] | undefined) ?? [];
  if (allowedRoles.length === 0) return true;

  const role = auth.getRole();
  if (!role) {
    router.navigateByUrl('/products');
    return false;
  }

  const ok = allowedRoles.map(r => r.toLowerCase()).includes(role.toLowerCase());
  if (ok) return true;

  router.navigateByUrl('/products');
  return false;
};
