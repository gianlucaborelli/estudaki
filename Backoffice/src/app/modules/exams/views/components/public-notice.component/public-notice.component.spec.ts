import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PublicNoticeComponent } from './public-notice.component';

describe('PublicNoticeComponent', () => {
  let component: PublicNoticeComponent;
  let fixture: ComponentFixture<PublicNoticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublicNoticeComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PublicNoticeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
