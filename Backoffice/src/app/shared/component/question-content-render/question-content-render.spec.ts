import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionContentRender } from './question-content-render';

describe('QuestionContentRender', () => {
  let component: QuestionContentRender;
  let fixture: ComponentFixture<QuestionContentRender>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionContentRender],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionContentRender);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
