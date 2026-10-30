import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs';
import { ToastService } from '../services/toast-service';
import { NavigationExtras, Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);
  const router = inject(Router);
  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const modelStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modelStateErrors.push(error.error.errors[key])
                }

              }
              // In JavaScript/TypeScript, throw isn't limited to Error objects
              throw modelStateErrors.flat();
            } else {
              toastService.error(error.error)
            }
            break;
          case 401:
            toastService.error("Unaothorized")
            break;
          case 404:
            router.navigateByUrl("/not-found") // route does not actually exist but the wild card route will be triggered which is what we want
            break;
          case 500:
            const navigationExtras: NavigationExtras = { state: { error: error.error } }
            router.navigateByUrl('/server-error', navigationExtras) // pass state to a path via NavigationExtras
            break;
          default:
            toastService.error("Something went wrong");
            break;
        }
      }
      throw error;
    })
  )
};
