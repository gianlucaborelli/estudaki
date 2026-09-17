import { Component, inject } from '@angular/core';
import { QuestionEditorDialogData } from './question-editor-dialog-data';
import { Question } from '../../../models/question';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { QuestionType } from '../../../models/question-type';
import { ContentEditor } from '../content-editor/content-editor';
import { FormControl, FormGroup, FormsModule } from '@angular/forms';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    ContentEditor,
    FormsModule
  ],
  selector: 'app-question-editor-dialog',
  styleUrl: './question-editor-dialog.css',
  templateUrl: './question-editor-dialog.html',
})
export class QuestionEditorDialog {
  private readonly dialogRef =
    inject(MatDialogRef<QuestionEditorDialog>);

  readonly data =
    inject<QuestionEditorDialogData>(MAT_DIALOG_DATA);

  question: Question;

  constructor() {
    this.question = this.cloneQuestion(
      this.data.question ?
    );
  }

  form = new FormGroup({
    content: new FormControl<string>('')
  });

  get isEditing(): boolean {
    return this.question.questionId !== null;
  }

  get dialogTitle(): string {
    return this.isEditing
      ? 'Editar questão'
      : 'Adicionar questão';
  }

  addChoice(): void {
    const nextOption =
      this.getNextOption();

    this.question.choices.push({
      option: nextOption,
      isCorrect: false,
      contentBlocks: []
    });
  }

  removeChoice(index: number): void {
    if (this.question.choices.length <= 1) {
      return;
    }

    this.question.choices.splice(index, 1);

    this.reorganizeOptions();
  }

  moveChoiceUp(index: number): void {
    if (index <= 0) {
      return;
    }

    const previous =
      this.question.choices[index - 1];

    this.question.choices[index - 1] =
      this.question.choices[index];

    this.question.choices[index] =
      previous;

    this.reorganizeOptions();
  }

  moveChoiceDown(index: number): void {
    if (
      index < 0 ||
      index >= this.question.choices.length - 1
    ) {
      return;
    }

    const next =
      this.question.choices[index + 1];

    this.question.choices[index + 1] =
      this.question.choices[index];

    this.question.choices[index] =
      next;

    this.reorganizeOptions();
  }

  addSubArea(): void {
    this.question.subAreas.push('');
  }

  removeSubArea(index: number): void {
    this.question.subAreas.splice(index, 1);
  }

  toggleSupport(supportId: string): void {
    const index =
      this.question.questionSupports.indexOf(
        supportId
      );

    if (index >= 0) {
      this.question.questionSupports.splice(index, 1);
      return;
    }

    this.question.questionSupports.push(
      supportId
    );
  }

  isSupportSelected(
    supportId: string
  ): boolean {
    return this.question.questionSupports.includes(
      supportId
    );
  }

  save(): void {
    if (!this.isValid()) {
      return;
    }

    this.dialogRef.close(
      this.question
    );
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private isValid(): boolean {
    if (
      this.question.questionNumber <= 0
    ) {
      return false;
    }

    if (
      this.isHtmlEmpty(
        this.question.questionContents
      )
    ) {
      return false;
    }

    return this.question.choices.every(
      choice =>
        !this.isHtmlEmpty(
          choice.content
        )
    );
  }

  private isHtmlEmpty(
    html: string | null | undefined
  ): boolean {
    if (!html) {
      return true;
    }

    const text =
      new DOMParser()
        .parseFromString(
          html,
          'text/html'
        )
        .body.textContent
        ?.trim();

    return !text;
  }

  private getNextOption(): string {
    const index =
      this.question.choices.length;

    return String.fromCharCode(
      65 + index
    );
  }

  private reorganizeOptions(): void {
    this.question.choices.forEach(
      (choice, index) => {
        choice.option =
          String.fromCharCode(
            65 + index
          );
      }
    );
  }

  private cloneQuestion(
    question: Question | null
  ): Question {
    if (!question) {
      return {
        questionId: '',
        questionNumber: 1,
        mainArea: '',
        subAreas: [],
        questionType: QuestionType.MultipleChoice,
        questionContents: [],
        choices: [
          {
            option: 'A',
            isCorrect: false,
            contentBlocks: []
          },
          {
            option: 'B',
            isCorrect: false,
            contentBlocks: []
          }
        ],
        questionSupports: [],
        isNullified: false,
        isPublished: false
      };
    }

    return structuredClone(question);
  }
}
