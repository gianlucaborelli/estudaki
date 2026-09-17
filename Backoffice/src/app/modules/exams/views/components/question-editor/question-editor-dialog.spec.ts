import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionEditorDialog } from '../question-editor-dialog/question-editor-dialog';

describe('QuestionEditorDialog', () => {
  let component: QuestionEditorDialog;
  let fixture: ComponentFixture<QuestionEditorDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionEditorDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionEditorDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
