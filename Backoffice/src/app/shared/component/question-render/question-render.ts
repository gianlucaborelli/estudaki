import { Component, computed, input, signal } from '@angular/core';
import { Question } from '../../../modules/exams/models/question';
import { MATERIAL_MODULES } from '../../imports/material.imports';
import { QuestionHeader } from '../question-header/question-header';
import { ContentRender } from '../content-render/content-render';

@Component({
  imports: [
    ...MATERIAL_MODULES,
    QuestionHeader,
    ContentRender
  ],
  selector: 'app-question-render',
  styleUrl: './question-render.css',
  templateUrl: './question-render.html',
})
export class QuestionRender {
  question = input.required<Question>();

  selectedChoices = signal<string[]>([]);

  hasSelection = computed(
    () => this.selectedChoices().length > 0
  );

  toggleChoice(option: string): void {
    this.selectedChoices.update(selected => {
      if (selected.includes(option)) {
        return selected.filter(value => value !== option);
      }

      return [...selected, option];
    });
  }

  isSelected(option: string): boolean {
    return this.selectedChoices().includes(option);
  }
}
