import { Pipe, PipeTransform } from '@angular/core';
import { ExamCategory } from '../../modules/exams/models/exam-category';


@Pipe({
  name: 'examCategory',
  standalone: true
})
export class ExamCategoryPipe implements PipeTransform {

  private readonly labels: Record<ExamCategory, string> = {
    [ExamCategory.BarExam]: 'Exame da Ordem',
    [ExamCategory.PublicServiceExam]: 'Concurso público',
    [ExamCategory.NationalExam]: 'ENEM',
    [ExamCategory.SchoolExam]: 'Exames escolares',
    [ExamCategory.UniversityEntranceExam]: 'Vestibular'
  };

  transform(value: ExamCategory | null | undefined): string {
    if (!value) {
      return 'Não definido';
    }

    return this.labels[value] ?? 'Não definido';
  }
}
