import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
  forwardRef,
  input
} from '@angular/core';
import Placeholder
  from '@tiptap/extension-placeholder';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { Editor } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';
import Image from '@tiptap/extension-image';

@Component({
  imports: [],
  selector: 'app-content-editor',
  styleUrl: './content-editor.css',
  templateUrl: './content-editor.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(
        () => ContentEditor
      ),
      multi: true
    }
  ]
})
export class ContentEditor

  implements
  AfterViewInit,
  OnDestroy,
  ControlValueAccessor {

  @ViewChild('editorElement', {
    static: true
  })
  private editorElement!: ElementRef<HTMLDivElement>;

  readonly placeholder =
    input('Digite o conteúdo...');

  private editor!: Editor;

  private value = '';

  private disabled = false;


  private onChange:
    (value: string) => void =
    () => { };

  private onTouched:
    () => void =
    () => { };



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

      content: this.value,

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

        const html =
          editor.getHTML();

        this.value = html;

        this.onChange(html);
      },

      onBlur: () => {

        this.onTouched();
      }
    });


    this.editor.setEditable(
      !this.disabled
    );
  }


  /* -------------------------------- */
  /* ControlValueAccessor             */
  /* -------------------------------- */

  writeValue(
    value: string | null
  ): void {

    this.value =
      value ?? '';

    if (!this.editor) {
      return;
    }

    const current =
      this.editor.getHTML();

    if (current === this.value) {
      return;
    }

    this.editor.commands.setContent(
      this.value || '<p></p>',
      {
        emitUpdate: false
      }
    );
  }


  registerOnChange(
    fn: (value: string) => void
  ): void {

    this.onChange = fn;
  }


  registerOnTouched(
    fn: () => void
  ): void {

    this.onTouched = fn;
  }


  setDisabledState(
    isDisabled: boolean
  ): void {

    this.disabled =
      isDisabled;

    if (this.editor) {

      this.editor.setEditable(
        !isDisabled
      );
    }
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
