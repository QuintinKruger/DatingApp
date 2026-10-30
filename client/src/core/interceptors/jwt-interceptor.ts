import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../services/account-service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);

  const user = accountService.currentUser();

  if (user) {
    req = req.clone({
      setHeaders: {
        "Authorization": `Bearer ${user.token}`
      }
    }); // req is imutable so we make a clone of it 
    
  }
  return next(req);
};
