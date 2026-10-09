import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { ExamService } from '../../../services/exam.service';
import { UploadExamFilesDialogData } from './upload-exam-files-dialog-data';

@Component({
  imports: [
    CommonModule,
    ...MATERIAL_MODULES,
    MatProgressSpinnerModule
  ],
  selector: 'app-upload-exam-files-dialog',
  styleUrl: './upload-exam-files-dialog.css',
  templateUrl: './upload-exam-files-dialog.html',
})
export class UploadExamFilesDialog {
  private readonly dialogRef =
    inject(MatDialogRef<UploadExamFilesDialog>);

  private readonly data =
    inject<UploadExamFilesDialogData>(MAT_DIALOG_DATA);

  private readonly examService =
    inject(ExamService);

  @ViewChild('examFileInput')
  private examFileInputRef!: ElementRef<HTMLInputElement>;

  @ViewChild('answerKeyFileInput')
  private answerKeyFileInputRef!: ElementRef<HTMLInputElement>;

  readonly examFile = signal<File | null>(null);
  readonly answerKeyFile = signal<File | null>(null);
  readonly isUploading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  get canSave(): boolean {
    return !!this.examFile() && !!this.answerKeyFile() && !this.isUploading();
  }

  browseExamFile(): void {
    this.examFileInputRef.nativeElement.click();
  }

  browseAnswerKeyFile(): void {
    this.answerKeyFileInputRef.nativeElement.click();
  }

  onExamFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';

    if (file) {
      this.examFile.set(file);
    }
  }

  onAnswerKeyFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';

    if (file) {
      this.answerKeyFile.set(file);
    }
  }

  save(): void {
    const examFile = this.examFile();
    const answerKeyFile = this.answerKeyFile();

    if (!examFile || !answerKeyFile) {
      return;
    }

    this.errorMessage.set(null);
    this.isUploading.set(true);

    this.examService.uploadExamFiles(
      this.data.publicNoticeId,
      this.data.examId,
      examFile,
      answerKeyFile
    ).subscribe({
      next: () => {
        this.isUploading.set(false);
        this.dialogRef.close(true);
      },
      error: error => {
        this.isUploading.set(false);

        this.errorMessage.set(
          'Não foi possível enviar os arquivos. Tente novamente.'
        );

        console.error(error);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
