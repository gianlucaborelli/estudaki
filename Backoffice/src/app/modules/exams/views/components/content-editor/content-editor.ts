import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
  effect,
  inject,
  input,
  model,
  signal
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import Placeholder
  from '@tiptap/extension-placeholder';
import { Editor } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';
import Image from '@tiptap/extension-image';
import { ImagePickerDialog } from '../image-picker-dialog/image-picker-dialog';


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
        })
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


  setParagraph(): void {

    this.editor
      .chain()
      .focus()
      .setParagraph()
      .run();
  }


  setHeading(
    level: 1 | 2 | 3
  ): void {

    this.editor
      .chain()
      .focus()
      .toggleHeading({ level })
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
