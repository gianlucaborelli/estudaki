import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MATERIAL_MODULES } from '../../../../../../shared/imports/material.imports';

import { ImagePickerDialogData } from './image-picker-dialog-data';
import { ExamService } from '../../../../services/exam.service';

@Component({
  imports: [
    CommonModule,
    ...MATERIAL_MODULES,
    MatProgressSpinnerModule
  ],
  selector: 'app-image-picker-dialog',
  styleUrl: './image-picker-dialog.css',
  templateUrl: './image-picker-dialog.html',
})
export class ImagePickerDialog {
  private readonly dialogRef =
    inject(MatDialogRef<ImagePickerDialog>);

  private readonly data =
    inject<ImagePickerDialogData>(MAT_DIALOG_DATA);

  private readonly examService = inject(ExamService);

  @ViewChild('fileInput')
  private fileInput!: ElementRef<HTMLInputElement>;

  readonly images = signal<string[]>(this.data.images ?? []);

  readonly selectedUrl = signal<string | null>(null);

  readonly isUploading =
    signal(false);

  readonly errorMessage =
    signal<string | null>(null);

  browseFiles(): void {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event): void {
    const input =
      event.target as HTMLInputElement;

    const file =
      input.files?.[0];

    input.value = '';

    if (file) {
      this.uploadFile(file);
    }
  }

  onPaste(event: ClipboardEvent): void {
    const items =
      event.clipboardData?.items;

    if (!items) {
      return;
    }

    for (const item of items) {
      if (!item.type.startsWith('image/')) {
        continue;
      }

      const file =
        item.getAsFile();

      if (file) {
        event.preventDefault();
        this.uploadFile(file);
      }

      return;
    }
  }

  select(url: string): void {
    this.selectedUrl.set(url);
  }

  confirm(): void {
    if (!this.selectedUrl()) {
      return;
    }

    this.dialogRef.close(
      this.selectedUrl()
    );
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private uploadFile(file: File): void {
    this.errorMessage.set(null);
    this.isUploading.set(true);

    // this.examService.uploadImage(file).subscribe({
    //   next: uploaded => {
    //     this.isUploading.set(false);

    //     // this.images.update(
    //     //   list => [...list, uploaded.url]
    //     // );

    //     // this.selectedUrl.set(uploaded.url);
    //   },
    //   error: () => {
    //     this.isUploading.set(false);

    //     this.errorMessage.set(
    //       'Não foi possível enviar a imagem. Tente novamente.'
    //     );
    //   }
    // });
  }
}
