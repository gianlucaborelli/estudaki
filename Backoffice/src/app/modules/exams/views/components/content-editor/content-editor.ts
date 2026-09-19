import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
  effect,
  inject,
  input,
  model
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import Placeholder
  from '@tiptap/extension-placeholder';
import { Editor, Extension } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';
import Image from '@tiptap/extension-image';
import Color from '@tiptap/extension-color';
import TextAlign from '@tiptap/extension-text-align';
import { TextStyle } from '@tiptap/extension-text-style';
import Underline from '@tiptap/extension-underline';
import { Table } from '@tiptap/extension-table';
import TableCell from '@tiptap/extension-table-cell';
import TableHeader from '@tiptap/extension-table-header';
import TableRow from '@tiptap/extension-table-row';
import { ImagePickerDialog } from '../image-picker-dialog/image-picker-dialog';

const FontSize = Extension.create({
  name: 'fontSize',

  addGlobalAttributes() {
    return [
      {
        types: ['textStyle'],
        attributes: {
          fontSize: {
            default: null,
            parseHTML: element => element.style.fontSize || null,
            renderHTML: attributes => attributes['fontSize']
              ? { style: `font-size: ${attributes['fontSize']}` }
              : {}
          }
        }
      }
    ];
  }
});

@Component({
  imports: [],
  selector: 'app-content-editor',
  styleUrl: './content-editor.css',
  templateUrl: './content-editor.html',
})
export class ContentEditor

  implements
  AfterViewInit,
  OnDestroy {

  @ViewChild('editorElement', {
    static: true
  })
  private editorElement!: ElementRef<HTMLDivElement>;

  readonly placeholder =
    input('Digite o conteúdo...');

  readonly content =
    model<string>('');

  readonly publicNoticeId =
    input<string>('');

  readonly fontSizes = [
    '8pt', '9pt', '10pt', '11pt', '12pt', '14pt',
    '16pt', '18pt', '20pt', '24pt', '28pt', '32pt', '36pt'
  ];

  private readonly dialog =
    inject(MatDialog);

  private editor!: Editor;

  constructor() {
    // Keeps the editor in sync when content() changes from outside (e.g. switching questions/choices)
    effect(() => {
      const value =
        this.content() || '<p></p>';

      if (!this.editor) {
        return;
      }

      if (this.editor.getHTML() === value) {
        return;
      }

      this.editor.commands.setContent(
        value,
        {
          emitUpdate: false
        }
      );
    });
  }

  ngAfterViewInit(): void {

    this.editor = new Editor({

      element:
        this.editorElement.nativeElement,

      extensions: [

        StarterKit,

        Placeholder.configure({
          placeholder: this.placeholder()
        }),

        Link.configure({
          openOnClick: false,
          autolink: true
        }),

        Image.configure({
          inline: false
        }),

        TextStyle,
        FontSize,
        Color,
        Underline,
        TextAlign.configure({
          types: ['heading', 'paragraph']
        }),
        Table.configure({
          resizable: true
        }),
        TableRow,
        TableHeader,
        TableCell
      ],

      content: this.content() || '<p></p>',

      editorProps: {
        attributes: {
          class:
            'format max-w-none focus:outline-none'
        },

        handleDOMEvents: {
          focus: () => false
        }
      },

      onUpdate: ({ editor }) => {

        this.content.set(
          editor.getHTML()
        );
      }
    });
  }


  /* -------------------------------- */
  /* Toolbar                           */
  /* -------------------------------- */

  toggleBold(): void {

    this.editor
      .chain()
      .focus()
      .toggleBold()
      .run();
  }


  toggleItalic(): void {

    this.editor
      .chain()
      .focus()
      .toggleItalic()
      .run();
  }


  toggleStrike(): void {

    this.editor
      .chain()
      .focus()
      .toggleStrike()
      .run();
  }

  toggleUnderline(): void {
    this.editor
      .chain()
      .focus()
      .toggleUnderline()
      .run();
  }


  toggleBulletList(): void {

    this.editor
      .chain()
      .focus()
      .toggleBulletList()
      .run();
  }


  toggleOrderedList(): void {

    this.editor
      .chain()
      .focus()
      .toggleOrderedList()
      .run();
  }


  toggleBlockquote(): void {

    this.editor
      .chain()
      .focus()
      .toggleBlockquote()
      .run();
  }


  setHorizontalRule(): void {

    this.editor
      .chain()
      .focus()
      .setHorizontalRule()
      .run();
  }

  setFontSize(event: Event): void {
    const fontSize =
      (event.target as HTMLSelectElement).value;

    this.editor
      .chain()
      .focus()
      .setMark('textStyle', { fontSize })
      .run();
  }

  setTextColor(event: Event): void {
    const color =
      (event.target as HTMLInputElement).value;

    this.editor
      .chain()
      .focus()
      .setColor(color)
      .run();
  }

  setTextAlign(alignment: 'left' | 'center' | 'right' | 'justify'): void {
    this.editor
      .chain()
      .focus()
      .setTextAlign(alignment)
      .run();
  }

  insertTable(): void {
    this.editor
      .chain()
      .focus()
      .insertTable({ rows: 3, cols: 3, withHeaderRow: true })
      .run();
  }

  addColumnAfter(): void {
    this.editor
      .chain()
      .focus()
      .addColumnAfter()
      .run();
  }

  addRowAfter(): void {
    this.editor
      .chain()
      .focus()
      .addRowAfter()
      .run();
  }

  deleteTable(): void {
    this.editor
      .chain()
      .focus()
      .deleteTable()
      .run();
  }

  clearFormatting(): void {
    this.editor
      .chain()
      .focus()
      .unsetAllMarks()
      .clearNodes()
      .run();
  }


  insertImage(): void {

    const dialogRef =
      this.dialog.open(ImagePickerDialog, {
        width: '640px',
        maxWidth: '95vw',
        data: {
          publicNoticeId: this.publicNoticeId()
        }
      });

    dialogRef.afterClosed().subscribe(
      (result?: string) => {

        if (!result) {
          return;
        }

        this.editor
          .chain()
          .focus()
          .setImage({ src: result })
          .run();
      }
    );
  }


  setLink(): void {

    const previousUrl =
      this.editor.getAttributes(
        'link'
      )['href'];

    const url =
      window.prompt(
        'URL',
        previousUrl ?? ''
      );

    if (url === null) {
      return;
    }

    if (url === '') {

      this.editor
        .chain()
        .focus()
        .unsetLink()
        .run();

      return;
    }

    this.editor
      .chain()
      .focus()
      .setLink({
        href: url
      })
      .run();
  }


  undo(): void {

    this.editor
      .chain()
      .focus()
      .undo()
      .run();
  }


  redo(): void {

    this.editor
      .chain()
      .focus()
      .redo()
      .run();
  }


  isActive(
    name: string
  ): boolean {

    return this.editor?.isActive(
      name
    ) ?? false;
  }


  isHeading(
    level: number
  ): boolean {

    return this.editor?.isActive(
      'heading',
      { level }
    ) ?? false;
  }


  ngOnDestroy(): void {

    this.editor?.destroy();
  }
}
