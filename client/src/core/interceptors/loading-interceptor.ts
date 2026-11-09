import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramString = params.keys().map(key => `${key}=${params.get(key)}`).join("&");
    return paramString ? `${url}?${paramString}` : url
  }

  const invalidateCache = (urlPattern: string) => {
    for(const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key)}
        console.log(`Cache invalidate for: ${key}`)
    }
  }

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method.includes('POST') && req.url.includes('/likes')) {
    invalidateCache('/likes')
  }
  if (req.method.includes('POST') && req.url.includes('/messages')) {
    invalidateCache('/messages')
  }
  if (req.method.includes('POST') && req.url.includes('/admin')) {
    invalidateCache('/admin')
  }
  if (req.method.includes('POST') && req.url.includes('/logout')) {
    cache.clear()
  }

  if (req.method === 'GET') {
    const cachedResponse = cache.get(cacheKey);
    if (cachedResponse) {
      return of(cachedResponse); // need to return an boservable because it is an intercept of http client that returns the same type
    } 
  }

  busyService.busy();

  return next(req).pipe(
    delay(500),
    tap(response => {
      cache.set(cacheKey, response) // set elements of the cache
    }),
    finalize(() => {
      busyService.idle()
    })
  )
};
