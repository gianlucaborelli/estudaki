import { Pipe, PipeTransform } from '@angular/core';
import { EducationLevel } from '../../modules/exams/models/education-level';

@Pipe({
  name: 'educationLevel',
  standalone: true
})
export class EducationLevelPipe implements PipeTransform {

  private readonly labels: Record<EducationLevel, string> = {
    [EducationLevel.Elementary]: 'Ensino fundamental',
    [EducationLevel.HighSchool]: 'Ensino médio',
    [EducationLevel.Undergraduate]: 'Ensino superior',
    [EducationLevel.Technical]: 'Ensino técnico'
  };

  transform(
    value: EducationLevel | null | undefined
  ): string {
    if (!value) {
      return 'Não definido';
    }

    return this.labels[value] ?? 'Não definido';
  }
}
