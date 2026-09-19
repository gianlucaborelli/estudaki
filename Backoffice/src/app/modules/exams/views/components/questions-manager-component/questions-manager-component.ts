import { Component, inject, Input, OnChanges, SimpleChanges, signal } from '@angular/core';
import { MATERIAL_MODULES } from '../../../../../shared/imports/material.imports';
import { PublicNotice } from '../../../models/publicNotice';
import { EducationLevelPipe } from '../../../../../shared/pipes/education-level.pipe';
import { QuestionTypePipe } from '../../../../../shared/pipes/question-type.pipe';
import { Exam } from '../../../models/exam';
import { Question, QuestionSupport } from '../../../models/question';
import { ExamService } from '../../../services/exam.service';
import { QuestionRender } from '../../../../../shared/component/question-render/question-render';
import { QuestionEditorDialog } from '../question-editor/question-editor-dialog';
import { QuestionSupportEditorDialog } from '../question-support-editor/question-support-editor-dialog';
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
export class QuestionsManagerComponent implements OnChanges {
  examService = inject(ExamService);

  @Input()
  publicNotice: PublicNotice | null = null;
  selectedExam: Exam | null = null;

  questionSupportDataSource = signal<QuestionSupport[]>([]);
  selectedQuestionSupport: QuestionSupport | null = null;

  imageDataSource = signal<string[]>([]);
  selectedImage: string | null = null;

  questionDataSource = signal<Question[]>([]);
  selectedQuestion: Question | null = null;

  private readonly dialog = inject(MatDialog);

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

  ngOnChanges(changes: SimpleChanges) {
    if (changes['publicNotice'] && this.publicNotice) {
      this.loadDate(this.publicNotice.id);
    }
  }

  onExamSelected(exam: Exam): void {
    this.selectedExam = exam;
    if (this.publicNotice && this.selectedExam) {
      this.loadQuestions(this.publicNotice.id, this.selectedExam.id)
    }
  }

  loadQuestions(publicNoticeId: string, examId: string) {
    this.examService.getQuestionsByExamId(publicNoticeId, examId).subscribe({
      next: questionDataSource => {
        this.questionDataSource.set(questionDataSource.items);
      }
    });
  }

  loadDate(publicNoticeId: string) {
    this.examService.getQuestionSupportByPublicNoticeIdPaged(
      publicNoticeId, 1, 2).subscribe({
        next: questionDataSource => {
          this.questionSupportDataSource.set(questionDataSource.items);
        }
      });

    this.examService.getImagesByPublicNoticeId(
      publicNoticeId).subscribe({
        next: images => {
          this.imageDataSource.set(images);
        }
      });
  }

  deleteQuestionSupport() { }

  openQuestionSupportEditorModal(addMode: boolean = true): void {
    if (!this.publicNotice) {
      return;
    }

    const questionSupport = addMode
      ? undefined
      : this.selectedQuestionSupport ?? undefined;

    const dialogRef = this.dialog.open(
      QuestionSupportEditorDialog,
      {
        width: '1200px',
        maxWidth: '95vw',
        height: '85vh',
        maxHeight: '95vh',
        autoFocus: false,
        data: {
          publicNotice: this.publicNotice,
          questionSupport
        }
      }
    );

    dialogRef.afterClosed().subscribe((result?: QuestionSupport) => {
      if (!result) {
        return;
      }

      this.questionSupportDataSource.update(questionSupports => {
        const index = questionSupports.findIndex(
          questionSupport => questionSupport.id === result.id
        );

        if (index < 0) {
          return [...questionSupports, result];
        }

        return questionSupports.map(questionSupport =>
          questionSupport.id === result.id
            ? result
            : questionSupport
        );
      });

      this.selectedQuestionSupport = result;
    });
  }

  onQuestionSupportRowClick(support: QuestionSupport): void {
    this.selectedQuestionSupport = support;
  }

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

  addImage() { }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      this.uploadImage(file);
    }

    input.value = '';
  }

  async addImageFromClipboard() {
    const clipboardItems = await navigator.clipboard.read();

    for (const item of clipboardItems) {
      const imageType = item.types.find(type => type.startsWith('image/'));

      if (!imageType) {
        continue;
      }

      const blob = await item.getType(imageType);
      const file = new File([blob], `clipboard-image.${imageType.split('/')[1]}`, { type: imageType });

      this.uploadImage(file);
      break;
    }
  }

  private uploadImage(file: File) {
    if (this.publicNotice) {
      this.examService.uploadImage(this.publicNotice.id, file).subscribe({
        next: imageUrl => {
          this.imageDataSource.update(images => [...images, imageUrl]);
        }
      });
    }
  }

  deleteImage() { }

  onImageClick(image: string) {
    this.selectedImage = this.selectedImage === image ? null : image;
  }
}
