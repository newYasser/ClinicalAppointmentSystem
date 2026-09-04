import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

const ABSOLUTE_URL = /^https?:\/\//i;


export const apiBaseUrlInterceptor: HttpInterceptorFn = (request, next) => {
  if (ABSOLUTE_URL.test(request.url)) {
    return next(request);
  }

  const path = request.url.replace(/^\/+/, '');
  return next(request.clone({ url: `${environment.apiBaseUrl}/${path}` }));
};
