import { Pipe, PipeTransform } from '@angular/core';
import { QuestionType } from '../../modules/exams/models/question-type';


@Pipe({
  name: 'questionType',
  standalone: true
})
export class QuestionTypePipe implements PipeTransform {

  private readonly labels: Record<QuestionType, string> = {
    [QuestionType.MultipleChoice]: 'Múltipla escolha',
    [QuestionType.OpenEnded]: 'Dissertativa',
    [QuestionType.Redaction]: 'Redação'
  };

  transform(value: QuestionType | null | undefined): string {
    if (!value) {
      return 'Não definido';
    }

    return this.labels[value] ?? 'Não definido';
  }
}
