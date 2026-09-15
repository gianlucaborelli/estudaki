import { Component, input } from '@angular/core';
import { Question } from '../../../modules/exams/models/question';
import { MATERIAL_MODULES } from '../../imports/material.imports';
import { ExamCategoryPipe } from '../../pipes/exam-category.pipe';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    ExamCategoryPipe
],
  selector: 'app-question-header',
  styleUrl: './question-header.css',
  templateUrl: './question-header.html',
})
export class QuestionHeader {
  question = input.required<Question>();

  downloadFile(url: string): void {
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  downloadQuestionBooklet(): void {
    const url = this.question().examBookletUrl;

    if (url) {
      this.downloadFile(url);
    }
  }

  downloadAnswerKey(): void {
    const url = this.question().answerKeyUrl;

    if (url) {
      this.downloadFile(url);
    }
  }
}
