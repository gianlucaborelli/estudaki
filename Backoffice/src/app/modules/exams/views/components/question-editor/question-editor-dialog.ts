import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuestionEditorDialogData } from './question-editor-dialog-data';
import { ExamCategory } from '../../../models/exam-category';
import { Question, QuestionSupport } from '../../../models/question';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { QuestionType } from '../../../models/question-type';
import { ContentEditor } from '../content-editor/content-editor';
import { FormsModule } from '@angular/forms';
import { ExamService } from '../../../services/exam.service';

@Component({
  imports: [
    CommonModule,
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

  readonly service = inject(ExamService);

  readonly QuestionType = QuestionType;

  question: Question;

  constructor() {
    this.question = this.cloneQuestion(
      this.data.question ?? null
    );
  }

  get isEditing(): boolean {
    return !!this.question.questionId;
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
      explanation: '',
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

  toggleSupport(support: QuestionSupport): void {
    const index =
      this.question.questionSupports.findIndex(
        s => s.id === support.id
      );

    if (index >= 0) {
      this.question.questionSupports.splice(index, 1);
      return;
    }

    this.question.questionSupports.push(
      support
    );
  }

  isSupportSelected(
    support: QuestionSupport
  ): boolean {
    return this.question.questionSupports.some(
      s => s.id === support.id
    );
  }

  save(): void {
    this.isEditing
      ? this.service.updateQuestion(
        this.data.publicNoticeId,
        this.data.examId,
        this.question.questionId,
        this.question
      ).subscribe({
        next: () => {
          // atualização concluída
        },
        error: error => {
          console.error(error);
        }
      })
      : this.service.createQuestion(
        this.data.publicNoticeId,
        this.data.examId,
        this.question
      ).subscribe({
        next: () => {
          // criação concluída
        },
        error: error => {
          console.error(error);
        }
      });

    this.dialogRef.close(this.question);
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
        this.question.statement
      )
    ) {
      return false;
    }

    return this.question.choices.every(
      choice =>
        !this.isHtmlEmpty(
          choice.explanation
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
        publicNoticeId: '',
        examId: '',
        publicNoticeNumber: '',
        year: new Date().getFullYear(),
        examinerOrganization: '',
        contractingOrganization: '',
        examCategory: ExamCategory.PublicServiceExam,
        phase: '',
        positions: [],
        area: '',
        educationLevel: '',
        publicNoticeFileUrl: '',
        examBookletUrl: '',
        answerKeyUrl: '',
        questionNumber: 1,
        mainArea: '',
        subAreas: [],
        questionType: QuestionType.MultipleChoice,
        statement: '',
        questionContents: [],
        choices: [
          {
            option: 'A',
            isCorrect: false,
            explanation: '',
            contentBlocks: []
          },
          {
            option: 'B',
            isCorrect: false,
            explanation: '',
            contentBlocks: []
          }
        ],
        questionSupports: [],
        isNullified: false,
        isPublished: false,
        createdAt: new Date().toISOString()
      };
    }

    return structuredClone(question);
  }
}
