import { inject, Service } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedResult } from '../../../shared/models/pagedResult';
import { Observable } from 'rxjs';
import { PublicNotice } from '../models/publicNotice';
import { Question, QuestionSupport } from '../models/question';

@Service()
export class ExamService {
  private http = inject(HttpClient);

  getPubicNoticeList(
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

  getImagesByPublicNoticeId(publicNoticeId: string): Observable<string[]> {
    return this.http.get<string[]>(
      `/api/exams/${publicNoticeId}/contents/images`,
      {
        withCredentials: true
      });
  }

  uploadImage(publicNoticeId: string, file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    console.log(file);
    console.log(formData)

    return this.http.post<string>(
      `/api/exams/${publicNoticeId}/contents/images`,
      formData,
      {
        withCredentials: true
      }
    );
  }

  getQuestionSupportByPublicNoticeIdPaged(
    publicNotice: string,
    pageNumber: number,
    pageSize: number,
    sortColumn?: string,
    sortDirection?: 'asc' | 'desc'): Observable<PagedResult<QuestionSupport>> {
    let params = new HttpParams()
      .set('page', pageNumber)
      .set('pageSize', pageSize);

    if (sortColumn) {
      params = params.set('sortLabel', sortColumn);
    }

    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PagedResult<QuestionSupport>>(
      `/api/exams/${publicNotice}/contents/questions-support`,
      {
        params,
        withCredentials: true
      }
    );
  }

  getPubicNoticeById(publicNoticeId: string): Observable<PublicNotice> {
    return this.http.get<PublicNotice>(
      `/api/exams/${publicNoticeId}`, {
      withCredentials: true
    });
  }

  getQuestionsByExamId(publicNoticeId: string, examId: string): Observable<PagedResult<Question>> {
    return this.http.get<PagedResult<Question>>(
      `/api/exams/${publicNoticeId}/exams/${examId}/questions`, {
      withCredentials: true
    });
  }
}
