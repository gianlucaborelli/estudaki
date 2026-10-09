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

  getPubicNoticeById(publicNoticeId: string): Observable<PublicNotice> {
    return this.http.get<PublicNotice>(
      `/api/exams/${publicNoticeId}`, {
      withCredentials: true
    });
  }

  getImagesByPublicNoticeId(publicNoticeId: string): Observable<string[]> {
    return this.http.get<string[]>(
      `/api/exams/${publicNoticeId}/contents/images`,
      {
        withCredentials: true
      });
  }

  uploadImage(
    publicNoticeId: string,
    file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<string>(
      `/api/exams/${publicNoticeId}/contents/images`,
      formData,
      {
        withCredentials: true
      }
    );
  }

  uploadExamFiles(
    publicNoticeId: string,
    examId: string,
    examFile: File,
    answerKeyFile: File): Observable<void> {
    const formData = new FormData();
    formData.append('publicNoticeId', publicNoticeId);
    formData.append('examId', examId);
    formData.append('examFile', examFile);
    formData.append('answerKeyFile', answerKeyFile);

    return this.http.post<void>(
      `/api/exams/${publicNoticeId}/exams/${examId}/contents/exam-files`,
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

  createQuestionSupport(
    publicNoticeId: string,
    questionSupport: QuestionSupport): Observable<QuestionSupport> {
    return this.http.post<QuestionSupport>(
      `/api/exams/${publicNoticeId}/contents/questions-support`,
      { questionSupportDto: questionSupport },
      {
        withCredentials: true
      }
    );
  }

  updateQuestionSupport(
    publicNoticeId: string,
    questionSupport: QuestionSupport): Observable<QuestionSupport> {
    return this.http.patch<QuestionSupport>(
      `/api/exams/${publicNoticeId}/contents/questions-support`,
      { questionSupport },
      {
        withCredentials: true
      }
    );
  }

  getQuestionsByExamId(
    publicNoticeId: string,
    examId: string,
    pageNumber: number,
    pageSize: number,
    sortColumn?: string,
    sortDirection?: 'asc' | 'desc'
  ): Observable<PagedResult<Question>> {
    let params = new HttpParams()
      .set('page', pageNumber)
      .set('pageSize', pageSize);

    if (sortColumn) {
      params = params.set('sortLabel', sortColumn);
    }

    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PagedResult<Question>>(
      `/api/exams/${publicNoticeId}/exams/${examId}/questions`,
      {
        params,
        withCredentials: true
      }
    );
  }

  createQuestion(
    publicNoticeId: string,
    examId: string,
    question: Question): Observable<Question> {
    return this.http.patch<Question>(
      `/api/exams/${publicNoticeId}/exams/${examId}/questions`,
      { question },
      {
        withCredentials: true
      }
    );
  }

  updateQuestion(
    publicNoticeId: string,
    examId: string,
    questionId: string,
    question: Question): Observable<Question> {
    return this.http.patch<Question>(
      `/api/exams/${publicNoticeId}/exams/${examId}/questions/${questionId}`,
      { question },
      {
        withCredentials: true
      }
    );
  }
}
