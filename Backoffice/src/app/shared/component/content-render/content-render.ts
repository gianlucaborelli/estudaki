import { Component, input, ViewEncapsulation } from '@angular/core';

/**
 * Renders raw Quill-generated HTML (Question.statement, Choice.explanation, QuestionSupport.content).
 * Uses shadow DOM so external CSS cannot style it and its content keeps the styling it was authored with.
 */
@Component({
    imports: [],
    selector: 'app-content-render',
    styleUrl: './content-render.css',
    templateUrl: './content-render.html',
    encapsulation: ViewEncapsulation.ShadowDom,
})
export class ContentRender {
    content = input.required<string>();
}
