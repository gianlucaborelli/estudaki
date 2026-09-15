import { Component, EventEmitter, Input, Output, output, signal } from '@angular/core';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { PublicNotice } from '../../../models/publicNotice';
import { Exam } from '../../../models/exam';
import { ExamCategoryPipe } from '../../../../../shared/pipes/exam-category.pipe';
import { EducationLevelPipe } from '../../../../../shared/pipes/education-level.pipe';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    ExamCategoryPipe,
    EducationLevelPipe
  ],
  selector: 'app-public-notice',
  styleUrl: './public-notice.component.css',
  templateUrl: './public-notice.component.html',
})
export class PublicNoticeComponent {

  @Input()
  publicNotice: PublicNotice | null = null;
  selectedExam = output<Exam>();
  examSelected = signal<Exam | null>(null);

  openEditPublicNoticeDialog() { }

  onSelectedExamChanged(exam: Exam): void {
    this.selectedExam.emit(exam);
  }

  openUploadImagesDialog() { }

  attachNotice() { }

  reviewQuestionsWithAI() { }
}
