import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  if (req.method === 'GET') {
    const cachedResponse = cache.get(req.url);
    if (cachedResponse) {
      return of(cachedResponse); // need to return an boservable because it is an intercept of http client that returns the same type
    } 
  }

  busyService.busy();

  return next(req).pipe(
    delay(500),
    tap(response => {
      cache.set(req.url, response) // set elements of the cache
    }),
    finalize(() => {
      busyService.idle()
    })
  )
};
