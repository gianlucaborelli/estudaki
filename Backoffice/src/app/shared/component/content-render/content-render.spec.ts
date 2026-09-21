import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContentRender } from './content-render';

describe('ContentRender', () => {
    let component: ContentRender;
    let fixture: ComponentFixture<ContentRender>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ContentRender],
        }).compileComponents();

        fixture = TestBed.createComponent(ContentRender);
        component = fixture.componentInstance;
        fixture.componentRef.setInput('content', '<p>Test</p>');
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
