import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionRender } from './question-render';

describe('QuestionRender', () => {
  let component: QuestionRender;
  let fixture: ComponentFixture<QuestionRender>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionRender],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionRender);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
