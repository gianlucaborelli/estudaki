import { inject, Service } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedResult } from '../../../shared/models/pagedResult';
import { Observable } from 'rxjs';
import { PublicNotice } from '../models/publicNotice';

@Service()
export class ExamService {
  private http = inject(HttpClient);

  getExamList(
    pageNumber: number,
    pageSize: number,
    sortColumn?: string,
    sortDirection?: 'asc' | 'desc'
  ): Observable<PagedResult<PublicNotice>> {
    let params = new HttpParams()
      .set('page', pageNumber)
      .set('pageSize', pageSize);

    if (sortColumn) {
      params = params.set('sortLabel', sortColumn);
    }

    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PagedResult<PublicNotice>>(
      '/api/exams',
      {
        params,
        withCredentials: true
      }
    );
  }
}
