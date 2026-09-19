import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { QuestionSupport } from '../../../models/question';
import { ContentEditor } from '../content-editor/content-editor';
import { QuestionSupportEditorDialogData } from './question-support-editor-dialog-data';

@Component({
  imports: [
    CommonModule,
    ...MATERIAL_MODULES,
    ContentEditor
  ],
  selector: 'app-question-support-editor-dialog',
  styleUrl: './question-support-editor-dialog.css',
  templateUrl: './question-support-editor-dialog.html',
})
export class QuestionSupportEditorDialog {
  private readonly dialogRef =
    inject(MatDialogRef<QuestionSupportEditorDialog>);

  readonly data =
    inject<QuestionSupportEditorDialogData>(MAT_DIALOG_DATA);

  questionSupport: QuestionSupport;

  constructor() {
    this.questionSupport = this.cloneQuestionSupport(
      this.data.questionSupport
    );
  }

  get isEditing(): boolean {
    return !!this.data.questionSupport;
  }

  get dialogTitle(): string {
    return this.isEditing
      ? 'Editar suporte de questão'
      : 'Adicionar suporte de questão';
  }

  save(): void {
    this.dialogRef.close(this.questionSupport);
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private cloneQuestionSupport(
    questionSupport?: QuestionSupport
  ): QuestionSupport {
    if (questionSupport) {
      return structuredClone(questionSupport);
    }

    return {
      id: crypto.randomUUID(),
      content: ''
    };
  }
}
