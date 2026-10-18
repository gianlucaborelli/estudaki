import { Component, inject, Input, OnChanges, SimpleChanges, signal, ViewChild } from '@angular/core';
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
import { ContentRender } from '../../../../../shared/component/content-render/content-render';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ImagePickerDialog } from '../image-picker-dialog/image-picker-dialog';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    EducationLevelPipe,
    QuestionTypePipe,
    QuestionRender,
    ContentRender
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
    'QuestionNumber',
    'ChoicesCount',
    'CorrectChoicesCount'
  ];

  questionSupportDisplayedColumns = [
    'id',
    'contentsCount'
  ];

  private questionPaginatorRef?: MatPaginator;
  private questionSortRef?: MatSort;
  private supportPaginatorRef?: MatPaginator;
  private supportSortRef?: MatSort;

  @ViewChild('questionPaginator') set questionPaginator(paginator: MatPaginator | undefined) {
    this.questionPaginatorRef = paginator;

    if (paginator) {
      paginator.page.subscribe(() => this.loadQuestions());
    }
  }

  @ViewChild('questionSort') set questionSort(sort: MatSort | undefined) {
    this.questionSortRef = sort;

    if (sort) {
      sort.sortChange.subscribe(() => {
        this.questionPaginatorRef?.firstPage();
        this.loadQuestions();
      });
    }
  }

  @ViewChild('supportPaginator') set supportPaginator(paginator: MatPaginator | undefined) {
    this.supportPaginatorRef = paginator;

    if (paginator) {
      paginator.page.subscribe(() => this.loadSupports());

      if (this.publicNotice) {
        this.loadSupports();
      }
    }
  }

  @ViewChild('supportSort') set supportSort(sort: MatSort | undefined) {
    this.supportSortRef = sort;

    if (sort) {
      sort.sortChange.subscribe(() => {
        this.supportPaginatorRef?.firstPage();
        this.loadSupports();
      });
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['publicNotice'] && this.publicNotice) {
      this.loadImages(this.publicNotice.id);

      if (this.supportPaginatorRef) {
        this.supportPaginatorRef.firstPage();
        this.loadSupports();
      }
    }
  }

  onExamSelected(exam: Exam): void {
    this.selectedExam = exam;
    if (this.publicNotice && this.selectedExam) {
      this.questionPaginatorRef?.firstPage();
      this.loadQuestions();
    }
  }

  loadQuestions(): void {
    if (!this.publicNotice || !this.selectedExam) {
      return;
    }

    const pageNumber = (this.questionPaginatorRef?.pageIndex ?? 0) + 1;
    const pageSize = this.questionPaginatorRef?.pageSize ?? 20;
    const sortColumn = this.questionSortRef?.active || undefined;
    const sortDirection = this.questionSortRef?.direction || undefined;

    this.isLoading = true;

    this.examService.getQuestionsByExamId(
      this.publicNotice.id,
      this.selectedExam.id,
      pageNumber,
      pageSize,
      sortColumn,
      sortDirection
    ).subscribe({
      next: result => {
        this.questionDataSource.set(result.items);

        if (this.questionPaginatorRef) {
          this.questionPaginatorRef.length = result.totalItems;
        }

        this.isLoading = false;
      }
    });
  }

  loadSupports(): void {
    if (!this.publicNotice) {
      return;
    }

    const pageNumber = (this.supportPaginatorRef?.pageIndex ?? 0) + 1;
    const pageSize = this.supportPaginatorRef?.pageSize ?? 20;
    const sortColumn = this.supportSortRef?.active || undefined;
    const sortDirection = this.supportSortRef?.direction || undefined;

    this.examService.getQuestionSupportByPublicNoticeIdPaged(
      this.publicNotice.id,
      pageNumber,
      pageSize,
      sortColumn,
      sortDirection
    ).subscribe({
      next: result => {
        this.questionSupportDataSource.set(result.items);

        if (this.supportPaginatorRef) {
          this.supportPaginatorRef.length = result.totalItems;
        }
      }
    });
  }

  private loadImages(publicNoticeId: string) {
    this.examService.getImagesByPublicNoticeId(
      publicNoticeId).subscribe({
        next: images => {
          this.imageDataSource.set(images);
        }
      });
  }

  deleteQuestionSupport() { }

  openQuestionSupportEditorModal(isEditorMode: boolean): void {
    if (!this.publicNotice) {
      return;
    }

    const questionSupport = isEditorMode
      ? this.selectedQuestionSupport
      : undefined;

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

    dialogRef
      .afterClosed()
      .subscribe((result?: QuestionSupport) => {
        if (!result) {
          return;
        }
        this.loadSupports();
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
              this.questionSupportDataSource(),
            examId: this.selectedExam?.id,
            publicNoticeId: this.publicNotice?.id
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

          this.loadQuestions();
          this.selectedQuestion = result;
        }
      );
  }

  openAddExistingQuestionIntoExamModal() { }

  onQuestionRowClick(question: Question) {
    this.selectedQuestion = question
  }

  async addImage() {
    const dialogRef = this.dialog.open(ImagePickerDialog, {
      width: '640px',
      maxWidth: '95vw',
      data: { publicNoticeId: this.publicNotice?.id }
    });

    dialogRef.afterClosed().subscribe((imageUrl?: string) => {
      if (!imageUrl) {
        return;
      }

      this.loadImages(this.publicNotice!.id)
    });
  }

  deleteImage() { }

  onImageClick(image: string) {
    this.selectedImage = this.selectedImage === image ? null : image;
  }
}
