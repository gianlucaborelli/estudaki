import { Component, Input } from '@angular/core';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { Exam } from '../../../models/exam';
import { EducationLevelPipe } from '../../../../../shared/pipes/education-level.pipe';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    EducationLevelPipe
  ],
  selector: 'app-exam-detail-component',
  styleUrl: './exam-detail-component.css',
  templateUrl: './exam-detail-component.html',
})
export class ExamDetailComponent {
  @Input()
  exam: Exam | null = null;

  openEditExamDialog() { }

  openUploadFileDialog() { }

  openQuestionUnifyDialog() { }
}
