import { Component, input } from '@angular/core';
import { BlockContent } from '../../../modules/exams/models/question';

@Component({
  imports: [],
  selector: 'app-question-content-render',
  styleUrl: './question-content-render.css',
  templateUrl: './question-content-render.html',
})
export class QuestionContentRender {
  content = input.required<BlockContent[]>();
}
