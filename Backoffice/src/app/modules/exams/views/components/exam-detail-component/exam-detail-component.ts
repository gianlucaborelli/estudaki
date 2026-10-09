import { Component, Input, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { Exam } from '../../../models/exam';
import { PublicNotice } from '../../../models/publicNotice';
import { EducationLevelPipe } from '../../../../../shared/pipes/education-level.pipe';
import { UploadExamFilesDialog } from '../upload-exam-files/upload-exam-files-dialog';

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

  @Input()
  publicNotice: PublicNotice | null = null;

  private readonly dialog = inject(MatDialog);

  openEditExamDialog() { }

  openUploadFileDialog(): void {
    if (!this.publicNotice || !this.exam) {
      return;
    }

    this.dialog.open(
      UploadExamFilesDialog,
      {
        width: '640px',
        maxWidth: '95vw',
        autoFocus: false,
        data: {
          publicNoticeId: this.publicNotice.id,
          examId: this.exam.id
        }
      }
    );
  }

  openQuestionUnifyDialog() { }
}
