import { PublicNotice } from '../../../models/publicNotice';
import { QuestionSupport } from '../../../models/question';

export interface QuestionSupportEditorDialogData {
  publicNotice: PublicNotice;
  questionSupport?: QuestionSupport;
}
