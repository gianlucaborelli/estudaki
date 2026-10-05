import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  PLATFORM_ID,
  ViewChild,
  effect,
  inject,
  input,
  model
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import type Quill from 'quill';
import { ImagePickerDialog } from '../image-picker-dialog/image-picker-dialog';

@Component({
  imports: [],
  selector: 'app-content-editor',
  styleUrl: './content-editor.css',
  templateUrl: './content-editor.html',
})
export class ContentEditor implements AfterViewInit, OnDestroy {
  @ViewChild('editorElement', { static: true })
  private editorElement!: ElementRef<HTMLDivElement>;

  readonly placeholder = input('Digite o conteúdo...');
  readonly content = model<string>('');
  readonly publicNoticeId = input<string>('');

  private readonly dialog = inject(MatDialog);
  private readonly platformId = inject(PLATFORM_ID);
  private quill?: Quill;

  constructor() {
    effect(() => {
      const value = this.content() || '';

      if (!this.quill || this.quill.root.innerHTML === value) {
        return;
      }

      this.setEditorContent(value);
    });
  }

  async ngAfterViewInit(): Promise<void> {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const [{ default: Quill }, katexModule] = await Promise.all([
      import('quill'),
      import('katex')
    ]);

    (window as Window & { katex: typeof katexModule.default }).katex =
      katexModule.default;

    this.quill = new Quill(this.editorElement.nativeElement, {
      theme: 'snow',
      placeholder: this.placeholder(),
      modules: {
        table: true,
        toolbar: {
          container: [
            ['bold', 'italic', 'underline', 'strike'],
            ['blockquote', 'code-block'],
            [{ header: 1 }, { header: 2 }],
            [{ list: 'ordered' }, { list: 'bullet' }, { list: 'check' }],
            [{ script: 'sub' }, { script: 'super' }],
            [{ indent: '-1' }, { indent: '+1' }],
            [{ direction: 'rtl' }],
            [{ size: ['small', false, 'large', 'huge'] }],
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            [{ color: [] }, { background: [] }],
            [{ font: [] }],
            [{ align: [] }],
            ['link', 'image', 'formula', 'table'],
            ['clean']
          ]
        }
      }
    });

    const toolbar = this.quill.getModule('toolbar') as {
      addHandler: (name: string, handler: () => void) => void;
    };

    toolbar.addHandler('image', () => this.insertImage());
    toolbar.addHandler('table', () => this.insertTable());

    this.setEditorContent(this.content());
    this.quill.on('text-change', () => this.content.set(this.quill?.root.innerHTML ?? ''));
  }

  ngOnDestroy(): void {
    this.quill?.off('text-change');
  }

  private setEditorContent(html: string): void {
    if (!this.quill) {
      return;
    }

    const delta = this.quill.clipboard.convert({ html });
    this.quill.setContents(delta, 'silent');
  }

  private insertImage(): void {
    const dialogRef = this.dialog.open(ImagePickerDialog, {
      width: '640px',
      maxWidth: '95vw',
      data: { publicNoticeId: this.publicNoticeId() }
    });

    dialogRef.afterClosed().subscribe((imageUrl?: string) => {
      if (!imageUrl || !this.quill) {
        return;
      }

      const selection = this.quill.getSelection(true);
      this.quill.insertEmbed(selection.index, 'image', imageUrl, 'user');
      this.quill.setSelection(selection.index + 1, 'silent');
    });
  }

  private insertTable(): void {
    const table = this.quill?.getModule('table') as {
      insertTable: (rows: number, columns: number) => void;
    } | undefined;

    table?.insertTable(3, 3);
  }
}
