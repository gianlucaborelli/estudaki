import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID, REQUEST } from '@angular/core';
import { isPlatformServer } from '@angular/common';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);

  if (isPlatformServer(platformId)) {
    const serverRequest = inject(REQUEST, { optional: true });
    let url = req.url;

    // Se for URL relativa no SSR, converte para URL absoluta usando a origem da requisição
    if (url.startsWith('/')) {
      const origin = serverRequest
        ? new URL(serverRequest.url).origin
        : `http://localhost:${process?.env?.['PORT'] || 4000}`;
      url = `${origin}${url}`;
    }

    // Encaminha os cookies recebidos pelo SSR para a chamada HTTP interna
    const cookieHeader = serverRequest?.headers.get('cookie');
    const headers = cookieHeader
      ? req.headers.set('Cookie', cookieHeader)
      : req.headers;

    const serverReq = req.clone({
      url,
      headers,
    });

    return next(serverReq);
  }

  // No Browser, garante que requisições para a API incluam os cookies
  if (req.url.startsWith('/api') && !req.withCredentials) {
    req = req.clone({ withCredentials: true });
  }

  return next(req);
};
