import { Component, inject, Input, signal } from '@angular/core';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { PublicNotice } from '../../../models/publicNotice';
import { EducationLevelPipe } from '../../../../../shared/pipes/education-level.pipe';
import { QuestionTypePipe } from '../../../../../shared/pipes/question-type.pipe';
import { Exam } from '../../../models/exam';
import { Question, QuestionSupport } from '../../../models/question';
import { ExamService } from '../../../services/exam.service';
import { QuestionRender } from '../../../../../shared/component/question-render/question-render';
import { QuestionEditorDialog } from '../question-editor/question-editor-dialog';
import { MatDialog } from '@angular/material/dialog';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    EducationLevelPipe,
    QuestionTypePipe,
    QuestionRender
  ],
  selector: 'app-questions-manager-component',
  styleUrl: './questions-manager-component.css',
  templateUrl: './questions-manager-component.html',
})
export class QuestionsManagerComponent {
  examService = inject(ExamService);

  @Input()
  publicNotice: PublicNotice | null = null;
  selectedExam: Exam | null = null;

  questionSupportDataSource = signal<QuestionSupport[]>([]);
  selectedQuestionSupport: QuestionSupport | null = null;

  questionDataSource = signal<Question[]>([]);
  selectedQuestion: Question | null = null;

  private readonly dialog =
    inject(MatDialog);

  isLoading: boolean = false

  questionDisplayedColumns = [
    'questionNumber',
    'questionType',
    'choicesCount',
    'correctChoicesCount'
  ];

  questionSupportDisplayedColumns = [
    'id',
    'contentsCount'
  ];

  onExamSelected(exam: Exam): void {
    this.selectedExam = exam;

    if (this.publicNotice && this.selectedExam) {
      this.loadDate(this.publicNotice.id, this.selectedExam.id)
    }
  }

  loadDate(publicNoticeId: string, examId: string) {
    this.examService.getQuestionsByExamId(publicNoticeId, examId).subscribe({
      next: questionDataSource => {
        this.questionDataSource.set(questionDataSource);
      }
    });
  }

  deleteQuestionSupport() { }

  openQuestionSupportEditorModal(addMode: boolean = true) { }

  onQuestionSupportRowClick(support: QuestionSupport) { }

  deleteQuestion() { }

  openQuestionEditorModal(question: Question | null): void {

    const dialogRef =
      this.dialog.open(
        QuestionEditorDialog,
        {
          width: '1200px',
          maxWidth: '95vw',
          maxHeight: '95vh',
          autoFocus: false,

          data: {
            question,

            availableQuestionSupports:
              this.questionSupportDataSource()
          }
        }
      );

    dialogRef
      .afterClosed()
      .subscribe(
        (result?: Question) => {

          if (!result) {
            return;
          }

          this.saveQuestion(result);
        }
      );
  }

  saveQuestion(question: Question) { }

  addNewQuestionEditorModalAsync() { }

  openAddExistingQuestionIntoExamModal() { }

  onQuestionRowClick(question: Question) {
    this.selectedQuestion = question
  }
}
