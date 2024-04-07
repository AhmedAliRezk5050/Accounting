import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from "../auth/services/auth.service";
import {inject} from "@angular/core";



export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  if(authService.isAuthenticatedSubject.value) {
    return true;
  } else {
    router.navigate(['/']);
    return false;
  }
};
