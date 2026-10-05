import { inject, Injectable, PLATFORM_ID, REQUEST } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class CookieService {
  private platformId = inject(PLATFORM_ID);
  private document = inject(DOCUMENT);
  private request = inject(REQUEST, { optional: true });

  /**
   * Obtém a string bruta de cookies tanto no ambiente SSR quanto no Browser.
   */
  private getRawCookies(): string {
    if (isPlatformServer(this.platformId)) {
      return this.request?.headers.get('cookie') ?? '';
    }
    return this.document.cookie || '';
  }

  /**
   * Obtém o valor de um cookie específico pelo nome.
   */
  getCookie(name: string): string | null {
    const rawCookies = this.getRawCookies();
    if (!rawCookies) {
      return null;
    }

    const nameLenPlus = name.length + 1;
    return (
      rawCookies
        .split(';')
        .map((c) => c.trim())
        .filter((cookie) => cookie.substring(0, nameLenPlus) === `${name}=`)
        .map((cookie) => decodeURIComponent(cookie.substring(nameLenPlus)))[0] || null
    );
  }

  /**
   * Verifica se existe um cookie específico ou qualquer cookie presente.
   */
  hasCookie(name?: string): boolean {
    const rawCookies = this.getRawCookies();
    if (!rawCookies) {
      return false;
    }

    if (!name) {
      return rawCookies.trim().length > 0;
    }

    return this.getCookie(name) !== null;
  }
}
