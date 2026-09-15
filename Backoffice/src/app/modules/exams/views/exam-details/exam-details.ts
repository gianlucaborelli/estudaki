import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PublicNotice } from '../../models/publicNotice';
import { ExamService } from '../../services/exam.service';
import { MATERIAL_MODULES } from '../../../../shared/imports/material.imports';
import { Exam } from '../../models/exam';
import { PublicNoticeComponent } from '../components/public-notice.component/public-notice.component';
import { ExamDetailComponent } from '../components/exam-detail-component/exam-detail-component';
import { QuestionsManagerComponent } from '../components/questions-manager-component/questions-manager-component';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    PublicNoticeComponent,
    ExamDetailComponent,
    QuestionsManagerComponent
  ],
  selector: 'app-exam-details',
  styleUrl: './exam-details.css',
  templateUrl: './exam-details.html',
})
export class ExamDetails {
  publicNoticeId: string = '';
  router = inject(ActivatedRoute);
  examService = inject(ExamService);

  publicNotice: PublicNotice | null = null;
  selectedExam: Exam | null = null;

  ngOnInit(): void {
    this.publicNoticeId = String(this.router.snapshot.paramMap.get('id'));
  }

  ngAfterViewInit(): void {
    this.examService.getPubicNoticeById(this.publicNoticeId).subscribe({
      next: publicNotice => {
        this.publicNotice = publicNotice;
      }
    });
  }

  onExamSelected(exam: Exam): void {
    this.selectedExam = exam;
    console.log(exam)
  }
}
