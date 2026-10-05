import { ComponentFixture, TestBed } from '@angular/core/testing';
import { QuestionHeader } from './question-header';

describe('QuestionHeader', () => {
  let component: QuestionHeader;
  let fixture: ComponentFixture<QuestionHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionHeader],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionHeader);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
